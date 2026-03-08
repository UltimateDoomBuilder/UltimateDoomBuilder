// A bunch of functions that I've built up
// They come in handy :>

#define PLAYERMAX 64
#define TEAMCOUNT 8
#define DEFAULTTID_SCRIPT 471

#define SECOND_TICS 35.714285714285715
#define UNIT_CM     2.73921568627451

#define DAMAGE_NORANDOM     0x40000000

int TeamNames[TEAMCOUNT] = 
{
    "Blue", "Red", "Green", "Gold", "Black", "White", "Orange", "Purple"
};

int TeamColors[TEAMCOUNT] = 
{
    CR_BLUE, CR_RED, CR_GREEN, CR_GOLD, CR_BLACK, CR_WHITE, CR_ORANGE, CR_PURPLE
};

int TeamColorCodes[TEAMCOUNT] = 
{
    "\ch", "\cg", "\cd", "\cf", "\cm", "\cj", "\ci", "\ct"
};

function int itof(int x) { return x << 16; }
function int ftoi(int x) { return x >> 16; }

function int abs(int x)
{
    if (x < 0) { return -x; }
    return x;
}

function int sign(int x)
{
    if (x < 0) { return -1; }
    return 1;
}

function int randSign(void)
{
    return (2*random(0,1))-1;
}

function int mod(int x, int y)
{
    int ret = x - ((x / y) * y);
    if (ret < 0) { ret = y + ret; }
    return ret;
}

function int pow(int x, int y)
{
    int n = 1;
    while (y-- > 0) { n *= x; }
    return n;
}

function int powFloat(int x, int y)
{
    int n = 1.0;
    while (y-- > 0) { n = FixedMul(n, x); }
    return n;
}

function int gcf(int a, int b)
{
    int c;
    while (1)
    {
        if (b == 0) { return a; }
        c = a % b;
        a = b;
        b = c;
    }
    
    return -1;
}

function int min(int x, int y)
{
    if (x < y) { return x; }
    return y;
}

function int max(int x, int y)
{
    if (x > y) { return x; }
    return y;
}

function int middle(int x, int y, int z)
{
    if ((x < z) && (y < z)) { return max(x, y); }
    return max(min(x, y), z);
}

function int percFloat(int intg, int frac)
{
    return itof(intg) + (itof(frac) / 100);
}

function int percFloat2(int intg, int frac1, int frac2)
{
    return itof(intg) + (itof(frac1) / 100) + (itof(frac2) / 10000);
}

function int keyUp(int key)
{
    int buttons = GetPlayerInput(-1, INPUT_BUTTONS);

    if ((~buttons & key) == key) { return 1; }
    return 0;
}

function int keyUp_any(int key)
{
    int buttons = GetPlayerInput(-1, INPUT_BUTTONS);

    if (~buttons & key) { return 1; }
    return 0;
}

function int keyDown(int key)
{
    int buttons = GetPlayerInput(-1, INPUT_BUTTONS);

    if ((buttons & key) == key) { return 1; }
    return 0;
}

function int keyDown_any(int key)
{
    int buttons = GetPlayerInput(-1, INPUT_BUTTONS);

    if (buttons & key) { return 1; }
    return 0;
}

function int keysPressed(void)
{
    int buttons     = GetPlayerInput(-1, INPUT_BUTTONS);
    int oldbuttons  = GetPlayerInput(-1, INPUT_OLDBUTTONS);
    int newbuttons  = (buttons ^ oldbuttons) & buttons;

    return newbuttons;
}

function int keyPressed(int key)
{
    if ((keysPressed() & key) == key) { return 1; }
    return 0;
}

function int keyPressed_any(int key)
{
    if (keysPressed() & key) { return 1; }
    return 0;
}

function int inputUp(int input)
{
    int buttons = GetPlayerInput(-1, MODINPUT_BUTTONS);

    if ((~buttons & input) == input) { return 1; }
    return 0;
}

function int inputUp_any(int input)
{
    int buttons = GetPlayerInput(-1, MODINPUT_BUTTONS);

    if (~buttons & input) { return 1; }
    return 0;
}

function int inputDown(int input)
{
    int buttons = GetPlayerInput(-1, MODINPUT_BUTTONS);

    if ((buttons & input) == input) { return 1; }
    return 0;
}

function int inputDown_any(int input)
{
    int buttons = GetPlayerInput(-1, MODINPUT_BUTTONS);

    if (buttons & input) { return 1; }
    return 0;
}

function int inputsPressed(void)
{
    int buttons     = GetPlayerInput(-1, MODINPUT_BUTTONS);
    int oldbuttons  = GetPlayerInput(-1, MODINPUT_OLDBUTTONS);
    int newbuttons  = (buttons ^ oldbuttons) & buttons;

    return newbuttons;
}

function int inputPressed(int input)
{
    if ((inputsPressed() & input) == input) { return 1; }
    return 0;
}

function int inputPressed_any(int input)
{
    if (inputsPressed() & input) { return 1; }
    return 0;
}

function int adjustBottom(int tmin, int tmax, int i)
{
    if (tmin > tmax)
    {
        tmax ^= tmin; tmin ^= tmax; tmax ^= tmin;  // XOR swap
    }

    if (i < tmin) { tmin = i; }
    if (i > tmax) { tmin += (i - tmax); }

    return tmin;
}

function int adjustTop(int tmin, int tmax, int i)
{
    if (tmin > tmax)
    {
        tmax ^= tmin; tmin ^= tmax; tmax ^= tmin;
    }

    if (i < tmin) { tmax -= (tmin - i); }
    if (i > tmax) { tmax = i; }

    return tmax;
}

function int adjustShort(int tmin, int tmax, int i)
{
    if (tmin > tmax)
    {
        tmax ^= tmin; tmin ^= tmax; tmax ^= tmin;
    }

    if (i < tmin)
    {
        tmax -= (tmin - i);
        tmin = i;
    }
    if (i > tmax)
    {
        tmin += (i - tmax);
        tmax = i;
    }
    
    return packShorts(tmin, tmax);
}


// Taken from http://zdoom.org/wiki/sqrt

function int sqrt_i(int number)
{
    if (number <= 3) { return number > 0; }

    int oldAns = number >> 1,                     // initial guess
        newAns = (oldAns + (number / oldAns)) >> 1; // first iteration

    // main iterative method
    while (newAns < oldAns)
    {
        oldAns = newAns;
        newAns = (oldAns + number / oldAns) >> 1;
    }

    return oldAns;
}

function int sqrt(int number)
{
    if (number == 1.0) { return 1.0; }
    if (number <= 0) { return 0; }
    int val = 150.0;
    for (int i=0; i<15; i++) { val = (val + FixedDiv(number, val)) >> 1; }

    return val;
}

function int magnitudeTwo(int x, int y)
{
    return sqrt_i(x*x + y*y);
}

function int magnitudeTwo_f(int x, int y)
{
    int len, ang;

    ang = VectorAngle(x, y);
    if (((ang + 0.125) % 0.5) > 0.25) { len = FixedDiv(y, sin(ang)); }
    else { len = FixedDiv(x, cos(ang)); }

    return len;
}

function int magnitudeThree(int x, int y, int z)
{
    return sqrt_i(x*x + y*y + z*z);
}

function int magnitudeThree_f(int x, int y, int z)
{
    int len, ang;

    ang = VectorAngle(x, y);
    if (((ang + 0.125) % 0.5) > 0.25) { len = FixedDiv(y, sin(ang)); }
    else { len = FixedDiv(x, cos(ang)); }

    ang = VectorAngle(len, z);
    if (((ang + 0.125) % 0.5) > 0.25) { len = FixedDiv(z, sin(ang)); }
    else { len = FixedDiv(len, cos(ang)); }

    return len;
}


function int quadPos(int a, int b, int c)
{
    int s1 = sqrt(FixedMul(b, b)-(4*FixedMul(a, c)));
    int s2 = (2 * a);
    int b1 = FixedDiv(-b + s1, s2);

    return b1;
}

function int quadNeg(int a, int b, int c)
{
    int s1 = sqrt(FixedMul(b, b)-(4*FixedMul(a, c)));
    int s2 = (2 * a);
    int b1 = FixedDiv(-b - s1, s2);

    return b1;
}

// All the arguments are to be fixed-point
function int quad(int a, int b, int c, int y)
{
    return FixedMul(a, FixedMul(y, y)) + FixedMul(b, y) + c + y;
}

function int quadHigh(int a, int b, int c, int x)
{
    return quadPos(a, b, c-x);
}

function int quadLow(int a, int b, int c, int x)
{
    return quadNeg(a, b, c-x);
}

function int inRange(int low, int high, int x)
{
    return ((x >= low) && (x < high));
}

function void AddAmmoCapacity(int type, int add)
{
    SetAmmoCapacity(type, GetAmmoCapacity(type) + add);
}

function int packShorts(int left, int right)
{
    return ((left & 0xFFFF) << 16) + (right & 0xFFFF);
}

function int leftShort(int packed) { return packed >> 16; }
function int rightShort(int packed) { return (packed << 16) >> 16; }


// This stuff only works with StrParam

function int cleanString(int string)
{
    int ret = "";
    int strSize = StrLen(string);

    int c, i, ignoreNext;
    
    for (i = 0; i < strSize; i++)
    {
        c = GetChar(string, i);

        if ( ( ((c > 8) && (c < 14)) || ((c > 31) && (c < 127)) || ((c > 160) && (c < 173)) ) && !ignoreNext)
        {
            ret = StrParam(s:ret, c:c);
        }
        else if (c == 28 && !ignoreNext)
        {
            ignoreNext = 1;
        }
        else
        {
            ignoreNext = 0;
        }
    }

    return ret;
}

function int cvarFromString(int prefix, int newname)
{
    int ret = "";
    int i, c;
    int prelen = strlen(prefix);
    int namelen = strlen(newname);
    int cap = prelen+namelen;

    for (i = 0; i <= cap; i++)
    {
        c = cond(i >= prelen, GetChar(newname, i-prelen), GetChar(prefix, i));

        if (
            (c > 64 && c < 91)  // is uppercase letter
         || (c > 90 && c < 123) // is lowercase letter
         || (c > 47 && c < 58)  // is number
         || c == 95             // _
         )
        {
            ret = StrParam(s:ret, c:c);
        }
    }

    return ret;
}

function int padStringR(int baseStr, int padChar, int len)
{
    int baseStrLen = StrLen(baseStr);
    int pad = "";
    int padLen; int i;

    if (baseStrLen >= len)
    {
        return baseStr;
    }
    
    padChar = GetChar(padChar, 0);
    padLen = len - baseStrLen;

    for (i = 0; i < padLen; i++)
    {
        pad = StrParam(s:pad, c:padChar);
    }

    return StrParam(s:baseStr, s:pad);
}

function int padStringL(int baseStr, int padChar, int len)
{
    int baseStrLen = StrLen(baseStr);
    int pad = "";
    int padLen; int i;

    if (baseStrLen >= len)
    {
        return baseStr;
    }
    
    padChar = GetChar(padChar, 0);
    padLen = len - baseStrLen;

    for (i = 0; i < padLen; i++)
    {
        pad = StrParam(s:pad, c:padChar);
    }

    return StrParam(s:pad, s:baseStr);
}

function int changeString(int string, int repl, int where)
{
    int i; int j; int k;
    int ret = "";
    int len = StrLen(string);
    int rLen = StrLen(repl);

    if ((where + rLen < 0) || (where >= len))
    {
        return string;
    }
    
    for (i = 0; i < len; i++)
    {
        if (inRange(where, where+rLen, i))
        {
            ret = StrParam(s:ret, c:GetChar(repl, i-where));
        }
        else
        {
            ret = StrParam(s:ret, c:GetChar(string, i));
        }
    }

    return ret;
}

function int sliceString(int string, int start, int end)
{
    int len = StrLen(string);
    int ret = "";
    int i;

    if (start < 0)
    {
        start = len + start;
    }

    if (end <= 0)
    {
        end = len + end;
    }

    start = max(0, start);
    end   = min(end, len-1);
    
    for (i = start; i < end; i++)
    {
        ret = StrParam(s:ret, c:GetChar(string, i));
    }

    return ret;
}

function int strcmp(int str1, int str2)
{
    int i,j,k,l;
    int len1 = StrLen(str1);
    int len2 = StrLen(str2);
    j = max(len1, len2);

    for (i = 0; i < j; i++)
    {
        if (i >= len1) { return -1; }
        if (i >= len2) { return  1; }
        
        k = GetChar(str1, i); l = GetChar(str2, i);

        if (k > j) { return  1; }
        if (k < j) { return -1; }
    }
    return 0;
}


// End StrParam

function int unusedTID(int start, int end)
{
    int ret = start - 1;
    int tidNum;

    if (start > end) { start ^= end; end ^= start; start ^= end; }  // good ol' XOR swap
    
    while (ret++ != end)
    {
        if (ThingCount(0, ret) == 0)
        {
            return ret;
        }
    }
    
    return -1;
}

function int getMaxHealth(void)
{
    int maxHP = GetActorProperty(0, APROP_SpawnHealth);

    if ((maxHP == 0) && (PlayerNumber() != -1))
    {
        maxHP = 100;
    }

    return maxHP;
}

function int giveHealth(int amount)
{
    return giveHealthFactor(amount, 1.0);
}

function int giveHealthFactor(int amount, int maxFactor)
{
    return giveHealthMax(amount, FixedMul(getMaxHealth(), maxFactor));
}

function int giveHealthMax(int amount, int maxHP)
{
    int newHP;

    int curHP = GetActorProperty(0, APROP_Health);

    if (maxHP == 0) { newHP = max(curHP, curHP+amount); }
    else
    {
        if (curHP > maxHP) { return 0; }
        newHP = middle(curHP, curHP+amount, maxHP);
    }

    SetActorProperty(0, APROP_Health, newHP);

    return newHP - curHP;
}

function int isDead(int tid)
{
    return GetActorProperty(tid, APROP_Health) <= 0;
}

function int isSinglePlayer(void)
{
    return GameType() == GAME_SINGLE_PLAYER;
}

function int isLMS(void)
{
    return GetCVar("lastmanstanding") || GetCVar("teamlms");
}

function int isCoop(void)
{
    int check1 = GameType() == GAME_NET_COOPERATIVE;
    int check2 = GetCVar("cooperative") || GetCVar("invasion") || GetCVar("survival");

    return check1 || check2;
}

function int isInvasion(void)
{
    return GetCVar("invasion");
}

function int isFreeForAll(void)
{
    if (GetCVar("terminator") || GetCVar("duel"))
    {
        return 1;
    }

    int check1 = GetCVar("deathmatch") || GetCVar("possession") || GetCVar("lastmanstanding");
    int check2 = check1 && !GetCVar("teamplay");

    return check2;
}

function int isTeamGame(void)
{
    int ret = (GetCVar("teamplay") || GetCVar("teamgame") || GetCVar("teamlms"));
    return ret;
}

function int spawnDistance(int item, int dist, int tid)
{
    int myX, myY, myZ, myAng, myPitch, spawnX, spawnY, spawnZ;

    myX = GetActorX(0); myY = GetActorY(0); myZ = GetActorZ(0);
    myAng = GetActorAngle(0); myPitch = GetActorPitch(0);

    spawnX = FixedMul(cos(myAng) * dist, cos(myPitch));
    spawnX += myX;
    spawnY = FixedMul(sin(myAng) * dist, cos(myPitch));
    spawnY += myY;
    spawnZ = myZ + (-sin(myPitch) * dist);

    return Spawn(item, spawnX, spawnY, spawnZ, tid, myAng >> 8);
}

function void SetInventory(int item, int amount)
{
    int count = CheckInventory(item);

    if (count == amount) { return; }
    
    if (count > amount)
    {
        TakeInventory(item, count - amount);
        return;
    }

    GiveAmmo(item, amount - count);
    return;
}
function int ToggleInventory(int inv)
{
    if (CheckInventory(inv))
    {
        TakeInventory(inv, 0x7FFFFFFF);
        return 0;
    }

    GiveInventory(inv, 1);
    return 1;
}

function void GiveAmmo(int type, int amount)
{
    if (GetCVar("sv_doubleammo"))
    {
        int m = GetAmmoCapacity(type);
        int expected = min(m, CheckInventory(type) + amount);

        GiveInventory(type, amount);
        TakeInventory(type, CheckInventory(type) - expected);
    }
    else
    {  
        GiveInventory(type, amount);
    }
}

function void GiveActorAmmo(int tid, int type, int amount)
{
    if (GetCVar("sv_doubleammo"))
    {
        int m = GetAmmoCapacity(type);
        int expected = min(m, CheckActorInventory(tid, type) + amount);

        GiveActorInventory(tid, type, amount);
        TakeActorInventory(tid, type, CheckActorInventory(tid, type) - expected);
    }
    else
    {  
        GiveActorInventory(tid, type, amount);
    }
}

function int cond(int test, int trueRet, int falseRet)
{
    if (test) { return trueRet; }
    return falseRet;
}

function int condTrue(int test, int trueRet)
{
    if (test) { return trueRet; }
    return test;
}

function int condFalse(int test, int falseRet)
{
    if (test) { return test; }
    return falseRet;
}

function void saveCVar(int cvar, int val)
{
    int setStr = StrParam(s:"set ", s:cvar, s:" ", d:val);
    int arcStr = StrParam(s:"archivecvar ", s:cvar);
    ConsoleCommand(setStr); ConsoleCommand(arcStr);
}

function int defaultCVar(int cvar, int defaultVal)
{
    int ret = GetCVar(cvar);
    if (ret == 0) { saveCVar(cvar, defaultVal); return defaultVal; }

    return ret;
}


function int onGround(int tid)
{
    return (GetActorZ(tid) - GetActorFloorZ(tid)) == 0;
}

function int ThingCounts(int start, int end)
{
    int i, ret = 0;

    if (start > end) { start ^= end; end ^= start; start ^= end; }
    for (i = start; i < end; i++) { ret += ThingCount(0, i); }

    return ret;
}

function int PlaceOnFloor(int tid)
{
    if (ThingCount(0, tid) != 1) { return 1; }
    
    SetActorPosition(tid, GetActorX(tid), GetActorY(tid), GetActorFloorZ(tid), 0);
    return 0;
}

#define DIR_E  1
#define DIR_NE 2
#define DIR_N  3
#define DIR_NW 4
#define DIR_W  5
#define DIR_SW 6
#define DIR_S  7
#define DIR_SE 8

function int getDirection(void)
{
    int sideMove = keyDown(BT_MOVERIGHT) - keyDown(BT_MOVELEFT);
    int forwMove = keyDown(BT_FORWARD) - keyDown(BT_BACK);

    if (sideMove || forwMove)
    {
        switch (sideMove)
        {
          case -1: 
            switch (forwMove)
            {
                case -1: return DIR_SW;
                case  0: return DIR_W;
                case  1: return DIR_NW;
            }
            break;

          case 0: 
            switch (forwMove)
            {
                case -1: return DIR_S;
                case  1: return DIR_N;
            }
            break;

          case 1: 
            switch (forwMove)
            {
                case -1: return DIR_SE;
                case  0: return DIR_E;
                case  1: return DIR_NE;
            }
            break;
        }
    }

    return 0;
}

function int isInvulnerable(void)
{
    int check1 = GetActorProperty(0, APROP_Invulnerable);
    int check2 = CheckInventory("PowerInvulnerable");

    return check1 || check2;
}

function void saveStringCVar(int string, int cvarname)
{
    int slen = StrLen(string);
    int i, c, cvarname2;

    for (i = 0; i < slen; i++)
    {
        cvarname2 = StrParam(s:cvarname, s:"_char", d:i);
        SaveCVar(cvarname2, GetChar(string, i));
    }

    while (1)
    {
        cvarname2 = StrParam(s:cvarname, s:"_char", d:i);
        c = GetCVar(cvarname2);

        if (c == 0) { break; }

        ConsoleCommand(StrParam(s:"unset ", s:cvarname2));
        i += 1;
    }
}

function int loadStringCVar(int cvarname)
{
    int ret = "";
    int i = 0, c, cvarname2;

    while (1)
    {
        cvarname2 = StrParam(s:cvarname, s:"_char", d:i);
        c = GetCVar(cvarname2);

        if (c == 0) { break; }

        ret = StrParam(s:ret, c:c);
        i += 1;
    }

    return ret;
}

function int defaultTID(int def)
{
    return _defaulttid(def, 0);
}

function int _defaulttid(int def, int alwaysPropagate)
{
    if (ClassifyActor(0) & ACTOR_WORLD) { return 0; }

    int tid = ActivatorTID();
    int i, changed = 0;

    if (ThingCount(0, tid) != 1)
    {
        tid = def;
        changed = 1;
        if (def <= 0)
        {
            i = random(12, 220);
            tid = unusedTID(i*100, (i+100)*100);
        }

        Thing_ChangeTID(0, tid);
    }

    if ((changed || (alwaysPropagate == 1)) && (alwaysPropagate != 2))
    {
        ACS_ExecuteAlways(DEFAULTTID_SCRIPT, 0, tid,0,0);
    }

    return tid;
}

script DEFAULTTID_SCRIPT (int tid) clientside
{
    if (ConsolePlayerNumber() == -1) { terminate; }
    Thing_ChangeTID(0, tid);
}

function int JumpZFromHeight(int height, int gravFactor)
{
    return sqrt(2 * height * gravFactor);
}

function int roundZero(int toround)
{
    int i = toround % 1.0;
    return ftoi(toround - i);
}

function int roundAway(int toround)
{
    int i = toround % 1.0;

    if (i == 0) { return ftoi(toround); }
    return ftoi(toround + (1.0 - i));
}

function int round(int toround)
{
    return ftoi(toround + 0.5);
}

function int ceil(int toround)
{
    return ftoi(toround + (1.0-1));
}

function int intFloat(int toround)
{
    return itof(ftoi(toround));
}

function int distance(int x1, int y1, int z1, int x2, int y2, int z2)
{
    return magnitudeThree_f(x2-x1, y2-y1, z2-z1);
}

function int distance_tid(int tid1, int tid2)
{
    int x1 = GetActorX(tid1);
    int y1 = GetActorY(tid1);
    int z1 = GetActorZ(tid1);

    int x2 = GetActorX(tid2);
    int y2 = GetActorY(tid2);
    int z2 = GetActorZ(tid2);

    return magnitudeThree_f(x2-x1, y2-y1, z2-z1);
}

function int distance_ftoi(int x1, int y1, int z1, int x2, int y2, int z2)
{
    return ftoi(distance(x1,y1,z1, x2,y2,z2));
}

function void printDebugInfo(void)
{
    int classify    = ClassifyActor(0);
    int fead        = classify & ACTOR_DEAD;
    int player      = classify & ACTOR_PLAYER;
    int pln         = PlayerNumber();

    Log(s:" -- DEBUG INFO -- ");

    Log(s:"Executed on tic ", d:Timer(), s:" on map ", d:GetLevelInfo(LEVELINFO_LEVELNUM));

    if (classify & (ACTOR_PLAYER | ACTOR_MONSTER))
    {
        Log(s:"Script activator has ", d:GetActorProperty(0, APROP_Health), s:"/", d:getMaxHealth(), s:" HP");
    }

    if (player)
    {
        Log(s:"Is player ", d:pln, s:" (", n:0, s:"\c-) with class number ", d:PlayerClass(pln));
    }

    Log(s:" -- END DEBUG -- ");
}


function int PlayerTeamCount(int teamNo)
{
    int i, ret;
    for (i = 0; i < PLAYERMAX; i++)
    {
        if (GetPlayerInfo(i, PLAYERINFO_TEAM) == teamNO) { ret++; }
    }
    return ret;
}

function int lower(int chr)
{
    if (chr > 64 && chr < 91) { return chr+32; }
    return chr;
}

function int upper(int chr)
{
    if (chr > 90 && chr < 123) { return chr-32; }
    return chr;
}

function int AddActorProperty(int tid, int prop, int amount)
{
    int newAmount = GetActorProperty(tid, prop) + amount;
    SetActorProperty(tid, prop, newAmount);
    return newAmount;
}

function int ClientCount(void)
{
    int ret, i;

    for (i = 0; i < PLAYERMAX; i++)
    {
        if (PlayerInGame(i) || PlayerIsSpectator(i)) { ret++; }
    }

    return ret;
}

function int HasRoom(int actorname, int x, int y, int z)
{
    int tid = unusedTID(40000, 50000);
    int ret = Spawn(actorname, x, y, z, tid);

    if (ret >= 1) { Thing_Remove(tid); }

    return ret;
}

function int RealPlayerCount(void)
{
    int ret, i;

    for (i = 0; i < PLAYERMAX; i++)
    {
        if (PlayerInGame(i) && !PlayerIsBot(i)) { ret++; }
    }

    return ret;
}
