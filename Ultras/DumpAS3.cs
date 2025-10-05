string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
string dumpFolder = System.IO.Path.Combine(
    System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),
        "AS3_Dumps"
    );
if (!System.IO.Directory.Exists(dumpFolder))
    System.IO.Directory.CreateDirectory(dumpFolder);

bot.Log("=== TARGETED DUMP START ===");
bot.Log($"World loaded: {bot.Flash.IsWorldLoaded}");

void SaveJson(string label, string json)
{
    if (string.IsNullOrWhiteSpace(json) || json == "null" || json == "{}")
    {
        bot.Log($"  {label}: non-serializable or empty (DisplayObject? circular?)");
        return;
    }
    string file = System.IO.Path.Combine(dumpFolder, $"{timestamp}_{label}.json");
    System.IO.File.WriteAllText(file, json);
    bot.Log($"  ✓ {label}: {json.Length} chars -> {file}");
}

void TryDump(string label, string path)
{
    try
    {
        if (bot.Flash.IsNull(path))
        {
            bot.Log($"  {label}: null on AS3 side (skipped)");
            return;
        }
        string json = bot.Flash.GetGameObject(path);
        SaveJson(label, json);
    }
    catch (System.Exception ex)
    {
        bot.Log($"  ✗ {label}: {ex.Message}");
    }
}

bot.Log("Probing non-serializable roots (expect empty):");
TryDump("rootClass", "rootClass");
TryDump("world", "world");
TryDump("ui", "ui");

bot.Log("\nDumping serializable leaves:");
TryDump("myAvatar", "world.myAvatar");
TryDump("myAvatar.objData", "world.myAvatar.objData");
TryDump("myAvatar.dataLeaf", "world.myAvatar.dataLeaf");
TryDump("myAvatar.dataLeaf.sta", "world.myAvatar.dataLeaf.sta");
TryDump("myAvatar.eqp", "world.myAvatar.objData.eqp");
TryDump("myAvatar.items", "world.myAvatar.items");
bot.Bank.Open();
bot.Bank.Load();
bot.Wait.ForTrue(() => bot.Bank.Items.Count > 0, 20);
TryDump("world.bankinfo.items", "world.bankinfo.items");
TryDump("myAvatar.houseitems", "world.myAvatar.houseitems");
TryDump("myAvatar.tempitems", "world.myAvatar.tempitems");
TryDump("myAvatar.factions", "world.myAvatar.factions");
TryDump("myAvatar.boosts", "world.myAvatar.boosts");

TryDump("sfc", "sfc");

TryDump("world.strMapName", "world.strMapName");
TryDump("world.lastArea", "world.lastArea");
TryDump("world.strFrame", "world.strFrame");

bot.Log("\n=== DUMP COMPLETE ===");
bot.Log($"Files saved to: {dumpFolder}");

try
{
    string nameRaw = bot.Flash.GetGameObject("world.myAvatar.pnm");
    bot.Log("Username (pnm raw): " + (string.IsNullOrEmpty(nameRaw) ? "NULL" : nameRaw));
    string uname = bot.Flash.GetGameObject("world.myAvatar.objData.strUsername");
    bot.Log("Username (objData.strUsername raw): " + (string.IsNullOrEmpty(uname) ? "NULL" : uname));
}
catch (System.Exception ex)
{
    bot.Log("Username test failed: " + ex.Message);
}
