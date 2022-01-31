using CodeImp.DoomBuilder.ZDoom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeImp.DoomBuilder.Rendering.Shaders
{
    internal class ShaderField
    {
        public int Line;
        public List<List<ZScriptToken>> ArrayDimensions;
        public string TypeName;
        public List<ZScriptToken> Initializer;
        public string Name;
    }

    internal class ShaderFunction
    {
        public int Line;
        public int CodeLine;
        public string ReturnValue;
        public string Name;
        public bool Override;
        public List<ZScriptToken> Arguments;
        public List<ZScriptToken> Code;
    }

    class Shader
    {
        internal ShaderGroup Group;
        internal string ParentName;
        public int CodeLine;
        public string Name;
        // data input for vertex shader
        public List<ShaderField> In;
        // data between vertex and fragment shader
        public List<ShaderField> V2F;
        // data output for fragment shader
        public List<ShaderField> Out;
        // source for main() of vertex shader
        public int SourceVertexLine;
        public List<ZScriptToken> SourceVertex;
        // source for main() of fragment shader
        public int SourceFragmentLine;
        public List<ZScriptToken> SourceFragment;
        // functions local to the shader/parents
        public List<ShaderFunction> Functions = new List<ShaderFunction>();

        private const string GLSLInternalSeparator = "_";

        public Shader(ShaderGroup group)
        {
            Group = group;
        }

        public ShaderFunction GetFunction(string identifier)
        {
            foreach (ShaderFunction func in Functions)
                if (func.Name == identifier)
                    return func;
            return null;
        }

        // dumps all uniforms into a string
        private string GetTokenListSource(List<ZScriptToken> tokens)
        {
            string ss = "";
            foreach (ZScriptToken tok in tokens)
            {
                bool isBlock = false;
                switch (tok.Type)
                {
                    case ZScriptTokenType.LineComment:
                        ss += "//";
                        break;
                    case ZScriptTokenType.BlockComment:
                        ss += "/*";
                        isBlock = true;
                        break;
                }
                ss += tok.Value;
                if (isBlock) ss += "*/";
            }
            return ss;
        }

        private string GetUniformSource()
        {

            string output = "";
            foreach (ShaderField field in Group.Uniforms)
            {

                output += string.Format("#line {0}\n", field.Line);

                output += "uniform ";
                output += field.TypeName;

                if (field.ArrayDimensions != null)
                {
                    foreach (List<ZScriptToken> arrayDim in field.ArrayDimensions)
                        output += "[" + GetTokenListSource(arrayDim) + "]";
                }

                output += " " + field.Name;

                if (field.Initializer != null)
                    output += GetTokenListSource(field.Initializer);

                output += ";\n";

            }

            return output;

        }

        private string GetDataIOInternalName(string block, string name)
        {
            return string.Format("{2}SC{2}B{0}{2}F{1}", block, name, GLSLInternalSeparator);
        }

        private string GetDataIOSource(string prefix, string blockid, List<ShaderField> block, bool vertexInput)
        {

            string output = "";
            foreach (ShaderField field in block)
            {

                output += string.Format("#line {0}\n", field.Line);

                if (vertexInput)
                {
                    int location;
                    switch (field.Name)
                    {
                        case "Position":
                            location = 0;
                            break;
                        case "Color":
                            location = 1;
                            break;
                        case "TextureCoordinate":
                            location = 2;
                            break;
                        case "Normal":
                            location = 3;
                            break;
                        default:
                            throw new ShaderCompileException("Invalid input field {0} (not supported)", field.Name);
                    }

                    output += string.Format("layout(location = {0}) ", location);
                }

                string typeName = field.TypeName;
                string flat = "";

                if (typeName.EndsWith("_flat"))
                {
                    flat = "flat ";
                    typeName = typeName.Remove(typeName.Length - 5, 5);
                }

                output += flat + prefix + " ";
                output += typeName;

                if (field.ArrayDimensions != null)
                {
                    foreach (List<ZScriptToken> arrayDim in field.ArrayDimensions)
                        output += "[" + GetTokenListSource(arrayDim) + "]";
                }

                output += " " + GetDataIOInternalName(blockid, field.Name);

                if (field.Initializer != null)
                    output += GetTokenListSource(field.Initializer);

                output += ";\n";

            }

            return output;

        }

        private void GetReferencedFunctions(List<ZScriptToken> source, List<string> functions)
        {

            for (int i = 0; i < source.Count; i++)
            {

                ZScriptToken token = source[i];
                if (token.Type != ZScriptTokenType.Identifier)
                    continue;

                if (functions.Contains(token.Value))
                    continue;

                // check token to the left - needs to not be identifier.
                // check token to the right - needs to be open parenthesis
                // ---
                // the idea is that we can differentiate pixel = desaturate(...) from vec4 desaturate(1,1,1,1)
                //
                ZScriptTokenType leftToken = ZScriptTokenType.Invalid;
                ZScriptTokenType rightToken = ZScriptTokenType.Invalid;

                for (int j = i-1; j >= 0; j--)
                {
                    ZScriptTokenType tt = source[j].Type;
                    if (!IsWhitespace(tt))
                    {
                        leftToken = tt;
                        break;
                    }
                }

                for (int j = i+1; j < source.Count; j++)
                {
                    ZScriptTokenType tt = source[j].Type;
                    if (!IsWhitespace(tt))
                    {
                        rightToken = tt;
                        break;
                    }
                }

                if (leftToken != ZScriptTokenType.Identifier && rightToken == ZScriptTokenType.OpenParen)
                {

                    // find function
                    functions.Add(token.Value);
                    // if function was found, recurse and find functions it may depend on.
                    ShaderFunction func = GetFunction(token.Value);
                    if (func == null) func = Group.GetFunction(token.Value);
                    if (func != null) GetReferencedFunctions(func.Code, functions);

                }

            }

        }

        private string GetFunctionSource(List<ZScriptToken> shaderSource)
        {

            List<string> functionNames = new List<string>();
            GetReferencedFunctions(shaderSource, functionNames);

            string preOutput = "";
            string output = "";

            foreach (string functionName in functionNames)
            {
                ShaderFunction func = GetFunction(functionName);
                if (func == null) func = Group.GetFunction(functionName);
                if (func == null) continue;

                string funcOutput = string.Format("#line {0}\n", func.Line) + func.ReturnValue + " " + func.Name + " (" + GetTokenListSource(func.Arguments) + ")";
                preOutput += funcOutput + ";\n";

                funcOutput += " {\n" + string.Format("#line {0}\n", func.CodeLine) + GetTokenListSource(func.Code) + "}\n";
                output += funcOutput;

            }

            return preOutput + "\n" + output;

        }

        private static bool IsWhitespace(ZScriptTokenType t)
        {
            switch (t)
            {
                case ZScriptTokenType.Whitespace:
                case ZScriptTokenType.LineComment:
                case ZScriptTokenType.BlockComment:
                case ZScriptTokenType.Newline:
                    return true;
            }
            return false;
        }

        public List<ZScriptToken> ReplaceIO(List<ZScriptToken> tokens)
        {

            List<ZScriptToken> output = new List<ZScriptToken>(tokens);

            // we replace all <structPrefix>.<field> with GetDataBlockInternalName(<structPrefix>, <field>)
            // thus, "in" and "out" are reserved keywords and may not be found out of such context
            for (int i = 0; i < output.Count; i++)
            {

                ZScriptToken token = output[i];
                if (token.Type != ZScriptTokenType.Identifier)
                    continue;
                if (token.Value != "in" && token.Value != "out" && token.Value != "v2f")
                    continue;

                string structPrefix = token.Value;

                int startIndex = i;

                i++;
                // skip whitespace...
                for (; i < output.Count; i++)
                    if (!IsWhitespace(output[i].Type)) break;

                if (i >= output.Count || output[i].Type != ZScriptTokenType.Dot)
                    continue;

                i++;
                // skip whitespace again...
                for (; i < output.Count; i++)
                    if (!IsWhitespace(output[i].Type)) break;

                if (i >= output.Count || output[i].Type != ZScriptTokenType.Identifier)
                    continue;

                //
                string fieldName = output[i].Value;
                string realName = GetDataIOInternalName(structPrefix, fieldName);
                // now, remove all tokens between prefix and current
                output.RemoveRange(startIndex + 1, i - startIndex);
                ZScriptToken realToken = new ZScriptToken(output[startIndex]);
                realToken.Value = realName;
                output[startIndex] = realToken;
                i = startIndex - 1;

                // check if this field exists, just in case. and produce an error
                List<ShaderField> searchIn = null;
                switch (structPrefix)
                {
                    case "in":
                        searchIn = In;
                        break;
                    case "out":
                        searchIn = Out;
                        break;
                    case "v2f":
                        searchIn = V2F;
                        break;
                }
                if (searchIn != null)
                {
                    bool found = false;
                    foreach (ShaderField field in searchIn)
                    {
                        if (field.Name == fieldName)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                        throw new ShaderCompileException("Referenced non-existent {0} field {1}", structPrefix, fieldName);
                }

            }

            return output;

        }

        public string GetVertexSource()
        {
            if (In == null || V2F == null || SourceVertex == null)
                throw new ShaderCompileException("Tried to compile incomplete shader {0}", Name);

            string src = "";
            src += GetDataIOSource("in", "in", In, true) + "\n\n";
            src += GetDataIOSource("out", "v2f", V2F, false) + "\n\n";
            src += GetUniformSource() + "\n";
            src += GetFunctionSource(SourceVertex) + "\n\n";
            src += "void main() {\n" + string.Format("#line {0}\n", SourceVertexLine) + GetTokenListSource(ReplaceIO(SourceVertex)) + "\n}\n\n";

            return src;
        }

        public string GetFragmentSource()
        {
            if (Out == null || V2F == null || SourceFragment == null)
                throw new ShaderCompileException("Tried to compile incomplete shader {0}", Name);

            string src = "";
            src += GetDataIOSource("in", "v2f", V2F, false) + "\n\n";
            src += GetDataIOSource("out", "out", Out, false) + "\n\n";
            src += GetUniformSource() + "\n";
            src += GetFunctionSource(SourceFragment) + "\n\n";
            src += "void main() {\n" + string.Format("#line {0}\n", SourceFragmentLine) + GetTokenListSource(ReplaceIO(SourceFragment)) + "\n}\n\n";

            return src;
        }
    }

    class ShaderGroup
    {
        internal List<ShaderField> Uniforms = new List<ShaderField>();
        internal List<Shader> Shaders = new List<Shader>();
        internal List<ShaderFunction> Functions = new List<ShaderFunction>();

        public Shader GetShader(string identifier)
        {
            foreach (Shader s in Shaders)
                if (s.Name == identifier) return s;
            return null;
        }

        public ShaderFunction GetFunction(string identifier)
        {
            foreach (ShaderFunction f in Functions)
                if (f.Name == identifier) return f;
            return null;
        }
    }

    class ShaderCompileException : Exception
    {
        public ShaderCompileException(string fmt, params object[] v) : base(string.Format(fmt, v)) { }
    }

    class ShaderCompiler
    {
        // this is to implement partial parsing. it counts {}, (), and []. it stops parsing at the specified type, if outside of nesting.
        // also if last block token equals to the type, it will stop at the outer level. (i.e. last })
        private static List<ZScriptToken> ReadEverythingUntil(ZScriptTokenizer t, ZScriptTokenType type, bool eofIsOk, bool skipWhitespace)
        {
            List<ZScriptToken> tokens = new List<ZScriptToken>();

            int levelCurly = 0;
            int levelSquare = 0;
            int levelParen = 0;

            while (true)
            {
                if (skipWhitespace)
                    t.SkipWhitespace();

                long cpos = t.Reader.BaseStream.Position;

                ZScriptToken token = t.ReadToken();
                if (token == null)
                {
                    if (!eofIsOk)
                        throw new ShaderCompileException("Expected {0} or token, got <EOF>", type);
                    break;
                }

                // if this is the end token, don't check anything -- just return
                if (levelCurly == 0 && levelSquare == 0 && levelParen == 0 && token.Type == type)
                {
                    // rewind and return token list
                    t.Reader.BaseStream.Position = cpos;
                    break;
                }

                switch (token.Type)
                {
                    case ZScriptTokenType.OpenCurly:
                        levelCurly++;
                        break;
                    case ZScriptTokenType.CloseCurly:
                        levelCurly--;
                        break;
                    case ZScriptTokenType.OpenParen:
                        levelParen++;
                        break;
                    case ZScriptTokenType.CloseParen:
                        levelParen--;
                        break;
                    case ZScriptTokenType.OpenSquare:
                        levelSquare++;
                        break;
                    case ZScriptTokenType.CloseSquare:
                        levelSquare--;
                        break;
                }

                tokens.Add(token);

            }

            return tokens;
        }

        private static void CompileShaderField(ShaderField field, ZScriptTokenizer t)
        {

            ZScriptToken token;
            // read name and array dimensions
            while (true)
            {
                t.SkipWhitespace();
                token = t.ExpectToken(ZScriptTokenType.OpenSquare, ZScriptTokenType.Identifier);
                if (!(token?.IsValid ?? true))
                    throw new ShaderCompileException("Expected array dimensions or field name, got {0}", token?.ToString() ?? "<EOF>");

                // array finished
                if (token.Type == ZScriptTokenType.Identifier)
                {
                    field.Name = token.Value;
                    break;
                }

                // read array
                List<ZScriptToken> arrayDimTokens = ReadEverythingUntil(t, ZScriptTokenType.CloseSquare, false, false);
                if (field.ArrayDimensions == null)
                    field.ArrayDimensions = new List<List<ZScriptToken>>();
                field.ArrayDimensions.Add(arrayDimTokens);

                token = t.ExpectToken(ZScriptTokenType.CloseSquare);
                if (!(token?.IsValid ?? true))
                    throw new ShaderCompileException("Expected closing square brace, got {0}", token?.ToString() ?? "<EOF>");
            }

            // read additional array dimensions if present, and initializer. or end parsing
            while (true)
            {
                t.SkipWhitespace();
                token = t.ExpectToken(ZScriptTokenType.OpenSquare, ZScriptTokenType.OpAssign, ZScriptTokenType.Semicolon);
                if (!(token?.IsValid ?? true))
                    throw new ShaderCompileException("Expected array dimensions, initializer or semicolon, got {0}", token?.ToString() ?? "<EOF>");

                // field is done
                if (token.Type == ZScriptTokenType.Semicolon)
                    break;

                // has initializer
                if (token.Type == ZScriptTokenType.OpAssign)
                {
                    field.Initializer = ReadEverythingUntil(t, ZScriptTokenType.Semicolon, false, false);
                    token = t.ExpectToken(ZScriptTokenType.Semicolon);
                    if (!(token?.IsValid ?? true))
                        throw new ShaderCompileException("Expected semicolon, got {0}", token?.ToString() ?? "<EOF>");
                    break;
                }

                // read array
                List<ZScriptToken> arrayDimTokens = ReadEverythingUntil(t, ZScriptTokenType.CloseSquare, false, false);
                if (field.ArrayDimensions == null)
                    field.ArrayDimensions = new List<List<ZScriptToken>>();
                field.ArrayDimensions.Add(arrayDimTokens);

                token = t.ExpectToken(ZScriptTokenType.CloseSquare);
                if (!(token?.IsValid ?? true))
                    throw new ShaderCompileException("Expected closing square brace, got {0}", token?.ToString() ?? "<EOF>");
            }

        }

        private static void CompileUniforms(ShaderGroup output, ZScriptTokenizer t)
        {

            // so a type of a variable is normally identifier+array dimensions
            // array dimensions may also exist on the variable itself (oh god this shitty C syntax)
            t.SkipWhitespace();
            ZScriptToken token;

            token = t.ExpectToken(ZScriptTokenType.OpenCurly);
            if (!(token?.IsValid ?? true))
                throw new ShaderCompileException("Expected uniforms block, got {0}", token?.ToString() ?? "<EOF>");

            while (true)
            {

                t.SkipWhitespace();
                token = t.ExpectToken(ZScriptTokenType.Identifier, ZScriptTokenType.CloseCurly);
                if (!(token?.IsValid ?? true))
                    throw new ShaderCompileException("Expected uniform or end of block, got {0}", token?.ToString() ?? "<EOF>");

                if (token.Type == ZScriptTokenType.CloseCurly)
                    break; // done reading uniforms

                // first goes the name, then array dimensions, then the variable name, then array dimensions, then initializer
                ShaderField field = new ShaderField();
                field.Line = t.PositionToLine(token.Position);
                field.TypeName = token.Value;

                CompileShaderField(field, t);

                // done reading field, add it
                output.Uniforms.Add(field);

            }

        }

        private static void CompileShaderFunction(ShaderFunction func, ZScriptTokenizer t)
        {
            ZScriptToken token;

            t.SkipWhitespace();
            token = t.ExpectToken(ZScriptTokenType.Identifier);
            if (!(token?.IsValid ?? true))
                throw new ShaderCompileException("Expected function name, got {0}", token?.ToString() ?? "<EOF>");

            func.Name = token.Value;

            t.SkipWhitespace();
            token = t.ExpectToken(ZScriptTokenType.OpenParen);
            if (!(token?.IsValid ?? true))
                throw new ShaderCompileException("Expected function argument list, got {0}", token?.ToString() ?? "<EOF>");

            func.Arguments = ReadEverythingUntil(t, ZScriptTokenType.CloseParen, false, false);

            token = t.ExpectToken(ZScriptTokenType.CloseParen);
            if (!(token?.IsValid ?? true))
                throw new ShaderCompileException("Expected end of function arguments, got {0}", token?.ToString() ?? "<EOF>");

            t.SkipWhitespace();
            token = t.ExpectToken(ZScriptTokenType.OpenCurly);
            if (!(token?.IsValid ?? true))
                throw new ShaderCompileException("Expected function code block, got {0}", token?.ToString() ?? "<EOF>");

            func.CodeLine = t.PositionToLine(token.Position);
            func.Code = ReadEverythingUntil(t, ZScriptTokenType.CloseCurly, false, false);

            token = t.ExpectToken(ZScriptTokenType.CloseCurly);
            if (!(token?.IsValid ?? true))
                throw new ShaderCompileException("Expected end of function code block, got {0}", token?.ToString() ?? "<EOF>");

        }

        private static void CompileFunctions(ShaderGroup output, ZScriptTokenizer t)
        {

            t.SkipWhitespace();
            ZScriptToken token;

            token = t.ExpectToken(ZScriptTokenType.OpenCurly);
            if (!(token?.IsValid ?? true))
                throw new ShaderCompileException("Expected functions block, got {0}", token?.ToString() ?? "<EOF>");

            while (true)
            {

                t.SkipWhitespace();
                token = t.ExpectToken(ZScriptTokenType.Identifier, ZScriptTokenType.CloseCurly);
                if (!(token?.IsValid ?? true))
                    throw new ShaderCompileException("Expected function or end of block, got {0}", token?.ToString() ?? "<EOF>");

                if (token.Type == ZScriptTokenType.CloseCurly)
                    break; // done reading functions

                bool isoverride = false;
                if (token.Value == "override")
                {
                    isoverride = true;

                    t.SkipWhitespace();
                    token = t.ExpectToken(ZScriptTokenType.Identifier);
                    if (!(token?.IsValid ?? true))
                        throw new ShaderCompileException("Expected function return type, got {0}", token?.ToString() ?? "<EOF>");
                }

                // <return value> <name> (<arguments>) { <code> }
                ShaderFunction func = new ShaderFunction();
                func.Line = t.PositionToLine(token.Position);
                func.ReturnValue = token.Value;
                func.Override = isoverride;

                CompileShaderFunction(func, t);

                // check if function with such name already exists in the shader
                // delete it.
                // overloading is not supported for now
                for (int i = 0; i < output.Functions.Count; i++)
                {
                    if (output.Functions[i].Name == func.Name)
                    {
                        if (!isoverride)
                            throw new ShaderCompileException("Function {0} is double-defined without 'override' keyword! (previous declaration at line {1})", func.Name, output.Functions[i].Line);
                        output.Functions.RemoveAt(i);
                        i--;
                        continue;
                    }
                }

                output.Functions.Add(func);

            }

        }

        private static void CompileShaderFunctions(Shader output, ZScriptTokenizer t)
        {

            t.SkipWhitespace();
            ZScriptToken token;

            token = t.ExpectToken(ZScriptTokenType.OpenCurly);
            if (!(token?.IsValid ?? true))
                throw new ShaderCompileException("Expected functions block, got {0}", token?.ToString() ?? "<EOF>");

            while (true)
            {

                t.SkipWhitespace();
                token = t.ExpectToken(ZScriptTokenType.Identifier, ZScriptTokenType.CloseCurly);
                if (!(token?.IsValid ?? true))
                    throw new ShaderCompileException("Expected function or end of block, got {0}", token?.ToString() ?? "<EOF>");

                if (token.Type == ZScriptTokenType.CloseCurly)
                    break; // done reading functions

                bool isoverride = false;
                if (token.Value == "override")
                {
                    isoverride = true;

                    t.SkipWhitespace();
                    token = t.ExpectToken(ZScriptTokenType.Identifier);
                    if (!(token?.IsValid ?? true))
                        throw new ShaderCompileException("Expected function return type, got {0}", token?.ToString() ?? "<EOF>");
                }

                // <return value> <name> (<arguments>) { <code> }
                ShaderFunction func = new ShaderFunction();
                func.Line = t.PositionToLine(token.Position);
                func.ReturnValue = token.Value;
                func.Override = isoverride;

                CompileShaderFunction(func, t);

                // check if function with such name already exists in the shader
                // delete it.
                // overloading is not supported for now
                if (!isoverride)
                {
                    ShaderFunction existingFunc = output.Group.GetFunction(func.Name);
                    if (existingFunc != null)
                        throw new ShaderCompileException("Function {0} is double-defined without 'override' keyword! (previous declaration at line {1})", func.Name, existingFunc.Line);
                }
                for (int i = 0; i < output.Functions.Count; i++)
                {
                    if (output.Functions[i].Name == func.Name)
                    {
                        if (!isoverride)
                            throw new ShaderCompileException("Function {0} is double-defined without 'override' keyword! (previous declaration at line {1})", func.Name, output.Functions[i].Line);
                        output.Functions.RemoveAt(i);
                        i--;
                        continue;
                    }
                }

                output.Functions.Add(func);

            }

        }

        private static List<ShaderField> CompileShaderDataBlock(ZScriptTokenizer t)
        {

            List<ShaderField> fields = new List<ShaderField>();

            t.SkipWhitespace();
            ZScriptToken token = t.ExpectToken(ZScriptTokenType.OpenCurly);
            if (!(token?.IsValid ?? true))
                throw new ShaderCompileException("Expected data block, got {0}", token?.ToString() ?? "<EOF>");

            while (true)
            {

                t.SkipWhitespace();
                token = t.ExpectToken(ZScriptTokenType.Identifier, ZScriptTokenType.CloseCurly);
                if (!(token?.IsValid ?? true))
                    throw new ShaderCompileException("Expected data field or end of block, got {0}", token?.ToString() ?? "<EOF>");

                if (token.Type == ZScriptTokenType.CloseCurly)
                    break;

                ShaderField field = new ShaderField();
                field.Line = t.PositionToLine(token.Position);
                field.TypeName = token.Value;

                CompileShaderField(field, t);

                fields.Add(field);

            }

            return fields;

        }

        private static List<ZScriptToken> CompileShaderSource(ZScriptTokenizer t)
        {

            // syntax: 
            //  fragment { ... code ... }
            // or
            //  vertex { ... code ... }

            t.SkipWhitespace();
            ZScriptToken token = t.ExpectToken(ZScriptTokenType.OpenCurly);
            if (!(token?.IsValid ?? true))
                throw new ShaderCompileException("Expected shader source block, got {0}", token?.ToString() ?? "<EOF>");

            List<ZScriptToken> tokens = ReadEverythingUntil(t, ZScriptTokenType.CloseCurly, false, false);

            token = t.ExpectToken(ZScriptTokenType.CloseCurly);
            if (!(token?.IsValid ?? true))
                throw new ShaderCompileException("Expected end of shader source block, got {0}", token?.ToString() ?? "<EOF>");

            return tokens;

        }

        private static void CompileShader(ShaderGroup output, ZScriptTokenizer t)
        {

            t.SkipWhitespace();

            ZScriptToken token = t.ExpectToken(ZScriptTokenType.Identifier);
            if (!(token?.IsValid ?? true))
                throw new ShaderCompileException("Expected shader identifier, got {0}", token?.ToString() ?? "<EOF>");

            t.SkipWhitespace();

            Shader s = new Shader(output);
            s.Name = token.Value;
            output.Shaders.Add(s);

            token = t.ExpectToken(ZScriptTokenType.Identifier, ZScriptTokenType.OpenCurly);
            if (!(token?.IsValid ?? true))
                throw new ShaderCompileException("Expected parent identifier or shader block, got {0}", token?.ToString() ?? "<EOF>");

            // has parent shader id
            if (token.Type == ZScriptTokenType.Identifier)
            {

                if (token.Value != "extends")
                    throw new ShaderCompileException("Expected 'extends', got {0}", token.ToString());

                t.SkipWhitespace();
                token = t.ExpectToken(ZScriptTokenType.Identifier);
                if (!(token?.IsValid ?? true))
                    throw new ShaderCompileException("Expected parent identifier, got {0}", token?.ToString() ?? "<EOF>");

                s.ParentName = token.Value;

                t.SkipWhitespace();
                token = t.ExpectToken(ZScriptTokenType.OpenCurly);
                if (!(token?.IsValid ?? true))
                    throw new ShaderCompileException("Expected shader block, got {0}", token?.ToString() ?? "<EOF>");

            }


            s.CodeLine = t.PositionToLine(token.Position);

            while (true)
            {

                t.SkipWhitespace();
                token = t.ExpectToken(ZScriptTokenType.Identifier, ZScriptTokenType.CloseCurly);
                if (!(token?.IsValid ?? true))
                    throw new ShaderCompileException("Expected shader sub-block or end of block, got {0}", token?.ToString() ?? "<EOF>");

                if (token.Type == ZScriptTokenType.CloseCurly)
                    break;

                switch (token.Value)
                {
                    case "in":
                        s.In = CompileShaderDataBlock(t);
                        break;
                    case "out":
                        s.Out = CompileShaderDataBlock(t);
                        break;
                    case "v2f":
                        s.V2F = CompileShaderDataBlock(t);
                        break;
                    case "functions":
                        CompileShaderFunctions(s, t);
                        break;
                    case "vertex":
                        s.SourceVertex = CompileShaderSource(t);
                        if (s.SourceVertex != null && s.SourceVertex.Count > 0)
                            s.SourceVertexLine = t.PositionToLine(s.SourceVertex[0].Position);
                        break;
                    case "fragment":
                        s.SourceFragment = CompileShaderSource(t);
                        if (s.SourceFragment != null && s.SourceFragment.Count > 0)
                            s.SourceFragmentLine = t.PositionToLine(s.SourceFragment[0].Position);
                        break;
                    default:
                        throw new ShaderCompileException("Expected shader sub-block, got {0}", token.ToString());
                }

            }

        }

        public static ShaderGroup Compile(string src)
        {
            ShaderGroup output = new ShaderGroup();

            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(src)))
            using (BinaryReader br = new BinaryReader(ms))
            {
                ZScriptTokenizer t = new ZScriptTokenizer(br);

                // main cycle
                // in the root scope, we allow three blocks:
                //  - uniforms{}
                //  - functions{}
                //  - shader <name> {}
                // everything else is a syntax error.
                while (true)
                {
                    t.SkipWhitespace();
                    ZScriptToken token = t.ExpectToken(ZScriptTokenType.Identifier);
                    if (token == null)
                        break;

                    if (!token.IsValid)
                        throw new ShaderCompileException("Expected 'uniforms', 'functions', or 'shader'; got {0}", token.ToString());

                    switch (token.Value)
                    {
                        case "uniforms":
                            CompileUniforms(output, t);
                            break;
                        case "functions":
                            CompileFunctions(output, t);
                            break;
                        case "shader":
                            CompileShader(output, t);
                            break;
                        default:
                            throw new ShaderCompileException("Expected 'uniforms', 'functions', or 'shader'; got {0}", token.ToString());
                    }
                }

                // done parsing, postprocess - apply parents
                foreach (Shader s in output.Shaders)
                {
                    List<string> parents = new List<string>();
                    parents.Add(s.Name);
                    Shader p = s;
                    while (p.ParentName != null && p.ParentName != "")
                    {
                        string parentName = p.ParentName;
                        if (parents.Contains(parentName))
                            throw new ShaderCompileException("Recursive parent shader {0} found", parentName);
                        parents.Add(parentName);
                        p = output.GetShader(parentName);
                        if (p == null)
                            throw new ShaderCompileException("Parent shader {0} not found", parentName);

                        if (s.In == null) s.In = p.In;
                        if (s.Out == null) s.Out = p.Out;
                        if (s.V2F == null) s.V2F = p.V2F;
                        if (s.SourceFragment == null)
                        {
                            s.SourceFragment = p.SourceFragment;
                            s.SourceFragmentLine = p.SourceFragmentLine;
                        }
                        if (s.SourceVertex == null)
                        {
                            s.SourceVertex = p.SourceVertex;
                            s.SourceVertexLine = p.SourceVertexLine;
                        }

                        // add functions from parent
                        foreach (ShaderFunction func in p.Functions)
                        {
                            if (s.GetFunction(func.Name) == null)
                                s.Functions.Add(func);
                        }
                    }
                }

                return output;
            }
        }
    }
}
