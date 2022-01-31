uniforms
{
	mat4 world;
	mat4 view;
	mat4 projection;
	mat4 modelnormal;
	vec4 campos;
	
	vec4 highlightcolor;
	vec4 stencilColor;
	float desaturation;

	vec4 fogsettings;
	vec4 fogcolor;
	vec4 sectorfogcolor;
	vec4 vertexColor;

	sampler2D texture1;
	sampler2D texture2;
	sampler2D texture3;

	// classic lighting related
	int drawPaletted;
	ivec2 colormapSize;
	int doomlightlevels;
	int sectorLightLevel;

	// dynamic light related
	vec4 lightPosAndRadius[64];
	vec4 lightOrientation[64]; // this is a vector that points in light's direction
	vec2 light2Radius[64]; // this is used with spotlights
	vec4 lightColor[64];
	float ignoreNormals;
	float lightsEnabled;

	// Slope handle length
	float slopeHandleLength;

}

functions
{
    vec4 getColorMappedColor(int entry, int depth)
    {
        vec2 uv = vec2((float(entry) + 0.5) / colormapSize.x, (float(depth) + 0.5) / colormapSize.y);
        vec4 colormapColor = texture(texture2, uv);
        return colormapColor;
    }
    
    int lightLevelFromVertexColor(vec3 color)
    {
        float result = max(max(color.r, color.g), color.b) * 255;
        if (result < 192 && doomlightlevels > 0) {
            // correct for darkening in Doom light mode
            result = -0.666667 * (-96 - result);
        }
        return int(result);
    }
        
    int classicLightLevelToColorMapOffset(int lightLevel, vec3 position, vec3 normal, bool hvmod)
    {
        const int LIGHTLEVELS = 16;
        const int LIGHTSEGSHIFT = 4;
        const int NUMCOLORMAPS = 32;
        const int MAXLIGHTSCALE = 48;
        const int DISTMAP = 2;
        
        int scaledLightLevel = lightLevel >> LIGHTSEGSHIFT;
        
        bool isFlat = abs(dot(normal, vec3(0, 0, 1))) > 1e-3; 
        
        if (hvmod) {
            if (abs(dot(normal, vec3(0, 1, 0))) < 1e-3)
            {
                scaledLightLevel++;
            }
            else if (abs(dot(normal, vec3(1, 0, 0))) < 1e-3)
            {
                scaledLightLevel--;
            }
        }
        
        int level;
        float dist = distance(position, campos.xyz);
        
        if (!isFlat) 
        {
            int startmap = int(((LIGHTLEVELS-1-scaledLightLevel)*2)*NUMCOLORMAPS/LIGHTLEVELS);
            
            // same calculation as Eternity Engine
            int index = int(2560.0 / dist);
            if (index >= MAXLIGHTSCALE) index = MAXLIGHTSCALE - 1;
            level = startmap - index / DISTMAP;
        }
        else
        {
            // same calculation as Eternity Engine
            float startmap = 2.0 * (30.0 - lightLevel / 8.0f);
            level = int(startmap - (1280.0f / dist)) + 1;
        }
        
       
        if (level < 0) level = 0;
        if (level >= NUMCOLORMAPS) level = NUMCOLORMAPS - 1;
        return level;
    }

	// This adds fog color to current pixel color
	vec4 getFogColor(vec3 PosW, vec4 color)
	{
		float fogdist = max(16.0, distance(PosW, campos.xyz));
		float fogfactor = exp2(campos.w * fogdist);

		color.rgb = mix(sectorfogcolor.rgb, color.rgb, fogfactor);
		return color;
	}

	vec4 desaturate(vec4 texel)
	{
		float gray = (texel.r * 0.3 + texel.g * 0.56 + texel.b * 0.14);	
		return vec4(mix(texel.rgb, vec3(gray), desaturation), texel.a);
	}

	vec3 getOneDynLightContribution(vec3 PosW, vec3 Normal, vec3 light, vec4 lColor, vec4 lPosAndRadius, vec4 lOrientation, vec2 l2Radius)
	{

		//is face facing away from light source?
		//      update 01.02.2017: offset the equation by 3px back to try to emulate GZDoom's broken visibility check.
		float diffuseContribution = dot(Normal, normalize(lPosAndRadius.xyz - PosW + Normal * 3.0));
		if (diffuseContribution < 0.0 && (ignoreNormals == 0 || (lColor.a > 0.979 && lColor.a < 0.981))) // attenuated light and facing away
			return light;
		
		diffuseContribution = max(diffuseContribution, 0.0); // to make sure

		//is pixel in light range?
		float dist = distance(PosW, lPosAndRadius.xyz);

		if (dist > lPosAndRadius.w)
			return light;

		float power = 1.0;
		power *= max(lPosAndRadius.w - dist, 0.0) / lPosAndRadius.w;

		if (lOrientation.w > 0.5)
		{
			vec3 lightDirection = normalize(lPosAndRadius.xyz - PosW);
			float cosDir = dot(lightDirection, lOrientation.xyz);
			float df = smoothstep(l2Radius.y, l2Radius.x, cosDir);
			power *= df;
		}

		if (lColor.a > 0.979 && lColor.a < 0.981) // attenuated light 98%
			power *= diffuseContribution;

		// for w/e reason GZDoom also does this
		power *= lColor.a;

		if (lColor.a >= 1)
			return light.rgb - lColor.rgb * power;

		return light.rgb + lColor.rgb * power;

	}

	vec4 getDynLightContribution(vec4 tcolor, vec4 baselight, vec3 PosW, vec3 Normal)
	{
		vec3 light = vec3(0, 0, 0);
		vec3 addlight = vec3(0, 0, 0);

		if (lightsEnabled != 0)
		{
			for (int i = 0; i < 64; i++)
			{
				if (lightColor[i].a == 0)
					break;
				if (lightColor[i].a < 0.4) // additive
					addlight = getOneDynLightContribution(PosW, Normal, addlight, lightColor[i], lightPosAndRadius[i], lightOrientation[i], light2Radius[i]);
				else light = getOneDynLightContribution(PosW, Normal, light, lightColor[i], lightPosAndRadius[i], lightOrientation[i], light2Radius[i]);
			}
		}

		return vec4(tcolor.rgb*(baselight.rgb+light)+addlight, tcolor.a*baselight.a);
	}
}

shader world3d_main
{
	in
	{
		vec3 Position;
		vec4 Color;
		vec2 TextureCoordinate;
		vec3 Normal;
	}

	v2f
	{
		vec4 Color;
		vec2 UV;
		vec3 PosW;
		vec3 Normal;
		vec4 viewpos;
		vec3_flat flatNormal;
		vec4_flat flatColor;
	}

	out
	{
		vec4 FragColor;
	}
	
	vertex
	{
		v2f.viewpos = view * world * vec4(in.Position, 1.0);
		gl_Position = projection * v2f.viewpos;
		v2f.PosW = (world * vec4(in.Position, 1.0)).xyz;
		v2f.Color = in.Color;
		v2f.UV = in.TextureCoordinate;
		v2f.Normal = normalize((modelnormal * vec4(in.Normal, 1.0)).xyz);
		v2f.flatColor = in.Color;
		v2f.flatNormal = normalize(in.Normal);
	}
	
	fragment
	{
		vec4 tcolor = texture(texture1, v2f.UV);
		tcolor = mix(tcolor, vec4(stencilColor.rgb, tcolor.a), stencilColor.a);
		tcolor = getDynLightContribution(tcolor, v2f.Color, v2f.PosW, v2f.Normal);
		out.FragColor = desaturate(tcolor);

		#if defined(ALPHA_TEST)
		if (out.FragColor.a < 0.5) discard;
		#endif

		if (fogsettings.x >= 0.0) out.FragColor = mix(out.FragColor, fogcolor, clamp((-v2f.viewpos.z - fogsettings.x) / (fogsettings.y - fogsettings.x), 0.0, 1.0));
	}
}

shader world3d_fullbright extends world3d_main
{
	fragment
	{
		vec4 tcolor = texture(texture1, v2f.UV);
		tcolor = mix(tcolor, vec4(stencilColor.rgb, tcolor.a), stencilColor.a);
		tcolor.a *= v2f.Color.a;
		out.FragColor = tcolor;

		#if defined(ALPHA_TEST)
		if (out.FragColor.a < 0.5) discard;
		#endif

		if (fogsettings.x >= 0.0) out.FragColor = mix(out.FragColor, fogcolor, clamp((-v2f.viewpos.z - fogsettings.x) / (fogsettings.y - fogsettings.x), 0.0, 1.0));
	}
}

shader world3d_main_highlight extends world3d_main
{
	fragment
	{
		vec4 tcolor = texture(texture1, v2f.UV);
		tcolor = mix(tcolor, vec4(stencilColor.rgb, tcolor.a), stencilColor.a);
		tcolor = getDynLightContribution(tcolor, v2f.Color, v2f.PosW, v2f.Normal);
		if (tcolor.a == 0.0)
		{
			out.FragColor = tcolor;
		}
		else
		{
			// Blend texture color and vertex color
			vec4 ncolor = desaturate(tcolor);

			out.FragColor = vec4(highlightcolor.rgb * highlightcolor.a + (ncolor.rgb - 0.4 * highlightcolor.a), max(v2f.Color.a + 0.25, 0.5));
		}

		#if defined(ALPHA_TEST)
		if (out.FragColor.a < 0.5) discard;
		#endif

		if (fogsettings.x >= 0.0) out.FragColor = mix(out.FragColor, fogcolor, clamp((-v2f.viewpos.z - fogsettings.x) / (fogsettings.y - fogsettings.x), 0.0, 1.0));
	}
}

shader world3d_fullbright_highlight extends world3d_fullbright
{
	fragment
	{
		vec4 tcolor = texture(texture1, v2f.UV);
		tcolor = mix(tcolor, vec4(stencilColor.rgb, tcolor.a), stencilColor.a);
		if(tcolor.a == 0.0)
		{
			out.FragColor = tcolor;
		}
		else
		{
			// Blend texture color and vertex color
			vec4 ncolor = tcolor * v2f.Color;

			out.FragColor = vec4(highlightcolor.rgb * highlightcolor.a + (tcolor.rgb - 0.4 * highlightcolor.a), max(v2f.Color.a + 0.25, 0.5));
		}

		#if defined(ALPHA_TEST)
		if (out.FragColor.a < 0.5) discard;
		#endif

		if (fogsettings.x >= 0.0) out.FragColor = mix(out.FragColor, fogcolor, clamp((-v2f.viewpos.z - fogsettings.x) / (fogsettings.y - fogsettings.x), 0.0, 1.0));
	}
}

shader world3d_vertex_color extends world3d_main
{
	fragment
	{
		out.FragColor = v2f.Color;

		#if defined(ALPHA_TEST)
		if (out.FragColor.a < 0.5) discard;
		#endif

		if (fogsettings.x >= 0.0) out.FragColor = mix(out.FragColor, fogcolor, clamp((-v2f.viewpos.z - fogsettings.x) / (fogsettings.y - fogsettings.x), 0.0, 1.0));
	}
}

shader world3d_main_vertexcolor extends world3d_main
{
	vertex
	{
		v2f.viewpos = view * world * vec4(in.Position, 1.0);
		gl_Position = projection * v2f.viewpos;
		v2f.Color = vertexColor;
		v2f.UV = in.TextureCoordinate;
	}
}

shader world3d_constant_color extends world3d_main_vertexcolor
{
	fragment
	{
		out.FragColor = vertexColor;

		#if defined(ALPHA_TEST)
		if (out.FragColor.a < 0.5) discard;
		#endif

		if (fogsettings.x >= 0.0f) out.FragColor = mix(out.FragColor, fogcolor, clamp((-v2f.viewpos.z - fogsettings.x) / (fogsettings.y - fogsettings.x), 0.0, 1.0));
	}
}

// world3d_main_highlight with vertex program from world3d_vertexcolor
// to-do: rewrite into a function
shader world3d_highlight_vertexcolor extends world3d_main_highlight
{
	vertex
	{
		v2f.viewpos = view * world * vec4(in.Position, 1.0);
		gl_Position = projection * v2f.viewpos;
		v2f.Color = vertexColor;
		v2f.UV = in.TextureCoordinate;
	}
}

shader world3d_main_fog extends world3d_main
{
	fragment
	{
		vec4 tcolor = texture(texture1, v2f.UV);
		tcolor = mix(tcolor, vec4(stencilColor.rgb, tcolor.a), stencilColor.a);
		tcolor = getDynLightContribution(tcolor, v2f.Color, v2f.PosW, v2f.Normal);
		if (tcolor.a == 0.0)
		{
			out.FragColor = tcolor;
		}
		else
		{
			out.FragColor = desaturate(getFogColor(v2f.PosW, tcolor));
		}

		#if defined(ALPHA_TEST)
		if (out.FragColor.a < 0.5) discard;
		#endif

		if (fogsettings.x >= 0.0) out.FragColor = mix(out.FragColor, fogcolor, clamp((-v2f.viewpos.z - fogsettings.x) / (fogsettings.y - fogsettings.x), 0.0, 1.0));
	}
}

shader world3d_main_highlight_fog extends world3d_main_fog
{
	fragment
	{
		vec4 tcolor = texture(texture1, v2f.UV);
		tcolor = mix(tcolor, vec4(stencilColor.rgb, tcolor.a), stencilColor.a);
		tcolor = vec4(getDynLightContribution(tcolor, v2f.Color, v2f.PosW, v2f.Normal).rgb, tcolor.a);
		if (tcolor.a == 0.0)
		{
			out.FragColor = tcolor;
		}
		else
		{
			// Blend texture color and vertex color
			vec4 ncolor = desaturate(getFogColor(v2f.PosW, tcolor));

			out.FragColor = vec4(highlightcolor.rgb * highlightcolor.a + (ncolor.rgb - 0.4 * highlightcolor.a), max(ncolor.a + 0.25, 0.5));
		}

		#if defined(ALPHA_TEST)
		if (out.FragColor.a < 0.5) discard;
		#endif

		if (fogsettings.x >= 0.0) out.FragColor = mix(out.FragColor, fogcolor, clamp((-v2f.viewpos.z - fogsettings.x) / (fogsettings.y - fogsettings.x), 0.0, 1.0));
	}
}

// world3d_fog, but with vertex shader from customvertexcolor_fog
// to-do: rewrite into a function
shader world3d_main_fog_vertexcolor extends world3d_main_fog
{
	vertex
	{
		v2f.viewpos = view * world * vec4(in.Position, 1.0);
		gl_Position = projection * v2f.viewpos;
		v2f.PosW = (world * vec4(in.Position, 1.0)).xyz;
		v2f.Color = vertexColor;
		v2f.UV = in.TextureCoordinate;
		v2f.Normal = normalize((modelnormal * vec4(in.Normal, 1.0)).xyz);
	}
}

shader world3d_main_highlight_fog_vertexcolor extends world3d_main_highlight_fog
{
	vertex
	{
		v2f.viewpos = view * world * vec4(in.Position, 1.0);
		gl_Position = projection * v2f.viewpos;
		v2f.PosW = (world * vec4(in.Position, 1.0)).xyz;
		v2f.Color = vertexColor;
		v2f.UV = in.TextureCoordinate;
		v2f.Normal = normalize((modelnormal * vec4(in.Normal, 1.0)).xyz);
	}
}

// Slope handle shader
shader world3d_slope_handle extends world3d_vertex_color
{
	vertex
	{
		v2f.viewpos = view * world * vec4(in.Position.x * slopeHandleLength, in.Position.y, in.Position.z, 1.0);
		gl_Position = projection * v2f.viewpos;
		v2f.Color = in.Color * vertexColor;
		v2f.UV = in.TextureCoordinate;
	}
}


shader world3d_classic extends world3d_main
{
	fragment
	{
        vec4 pcolor;
		 
		if (bool(drawPaletted))
		{
		    vec4 color = texture(texture1, v2f.UV);
		    int entry = int(color.r * 255);
		    float alpha = color.a;
		    int lightLevel = lightLevelFromVertexColor(v2f.flatColor.rgb);
            int colorMapOffset = classicLightLevelToColorMapOffset(lightLevel, v2f.PosW, v2f.flatNormal, false);
            pcolor = getColorMappedColor(entry, colorMapOffset);
            pcolor.a = alpha;
		}
		else
		{
            pcolor = texture(texture1, v2f.UV);
		}
		
		out.FragColor = pcolor;
		
		#if defined(ALPHA_TEST)
		if (out.FragColor.a < 0.5) discard;
		#endif
	}
}

shader world3d_classic_highlight extends world3d_main
{
	fragment
	{
		vec4 pcolor;
		 
		if (bool(drawPaletted))
        {
            vec4 color = texture(texture1, v2f.UV);
            int entry = int(color.r * 255);
            float alpha = color.a;
            int lightLevel = lightLevelFromVertexColor(v2f.flatColor.rgb);
            int modifiedLightLevel = max(lightLevel, 128);	 
            int colorMapOffset = classicLightLevelToColorMapOffset(modifiedLightLevel, v2f.PosW, v2f.flatNormal, false);
            pcolor = getColorMappedColor(entry, colorMapOffset);
            pcolor.a = alpha;
        }
        else
        {
            pcolor = texture(texture1, v2f.UV);
        }
		
		out.FragColor = pcolor;
		
		if (pcolor.a > 0.0)
		{
			out.FragColor = vec4(highlightcolor.rgb * highlightcolor.a + (pcolor.rgb - 0.4 * highlightcolor.a), max(pcolor.a + 0.25, 0.5));
		}
		
		#if defined(ALPHA_TEST)
		if (out.FragColor.a < 0.5) discard;
		#endif
	}
}