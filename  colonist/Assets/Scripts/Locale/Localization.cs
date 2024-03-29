using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;


/// <summary>
/// LocalizeCharacter wrap data on the portrait of dialogue.
/// </summary>
public class LocalizeCharacter
{
    public string CharacterID = "";
    public string CharacterName = "";
    /// <summary>
    /// The character icon path. Normally it's something like "Portrait\William"
    /// </summary>
    public string CharacterIconPath = "";
    /// <summary>
    /// The icon texture loaded from CharacterIconPath.
    /// </summary>
    public Texture2D PortraitIconTexture = null;
}

/// <summary>
/// LocalizedDialogueItem wrap data on a dialog script.
/// </summary>
public class LocalizedDialogueItem
{
	/// <summary>
	/// The name of the dialog image.
	/// It's at this pattern, for example:
	/// Asset/Resource/Language/Dialog/Level00/Chinese/0
	/// Asset/Resource/Language/Dialog/Level01/Chinese/0
	/// </summary>
    public string DialogImageName;
    /// <summary>
    /// The character ID who speak the dialog. Mapped to Localization_Character.CharacterID
    /// </summary>
    public string DialogCharacterID = "";

    /// <summary>
    /// pendAfterFinished = after dialog typed complete, how many seconds should be waited.
    /// if pendAfterFinished = null, use default setting in GameDialogue.
    /// </summary>
    public float? pendAfterFinished = null;

}

/// <summary>
/// LocalizedDialogue wrap data on a dialog.It contains several LocalizedDialogueItem, and display the LocalizedDialogueItem one by one.
/// </summary>
public class LocalizedDialogue
{
    public string DialogID = "";
    public IList<LocalizedDialogueItem> dialogueItem = new List<LocalizedDialogueItem>();
}

/// <summary>
/// Localization handles the localization business.
/// Current support lanuage: 
///   1. Chinese, 2. English, 
/// </summary>
public class Localization
{

    public const string DialogAssetRootFolder = "Language/Dialog";
    public const string CharacterAssetRootFolder = "Language/Character/Character";
    public const string SupportLanguage = "English;Chinese;";

    static IDictionary<string, LocalizeCharacter> CharacterDict = new Dictionary<string, LocalizeCharacter>();
    static IDictionary<string, LocalizedDialogue> DialogueDict = new Dictionary<string, LocalizedDialogue>();

    static SystemLanguage TargetLanguage;

    /// <summary>
    /// Initialize level dialogue by the specified language.
    /// if
    /// </summary>
    public static void InitializeLevelDialogue(string LevelName, SystemLanguage? _language)
    {
        TargetLanguage = _language == null ? SystemLanguage.English :
                          (SupportLanguage.Contains(_language.Value.ToString()) ? _language.Value : SystemLanguage.English);

        string DialogFilePath = DialogAssetRootFolder + "/" + LevelName + "/" + TargetLanguage.ToString() + "/" + TargetLanguage.ToString();
        TextAsset dialogFile = Resources.Load(DialogFilePath, typeof(TextAsset)) as TextAsset;
        TextAsset characterXMLFile = Resources.Load(CharacterAssetRootFolder, typeof(TextAsset)) as TextAsset;
        CharacterDict.Clear();
        DialogueDict.Clear();
        CharacterDict = ParseCharacterLocalizationXMLFile(ParseTextAssetToXMLDocument(characterXMLFile), TargetLanguage);
        DialogueDict = ParseDialogLocalizationXMLFile(ParseTextAssetToXMLDocument(dialogFile));
    }

    static XmlDocument ParseTextAssetToXMLDocument(TextAsset textasset)
    {
        XmlDocument xmlDoc = new XmlDocument();
        //because of annoying feature of Unity, that the way to read UTF-8 XML, we need to skip BOM(byte order mark)
        //but for XML without UTF-8 character, we MUST NOT skip first character.
        //so, we firstly not skip BOM, try to load XML, if it fail, then try skip BOM to parse again.
        bool parseOK = false;
        //1. not SKIP first character
        try
        {
            xmlDoc.LoadXml(textasset.text);
            parseOK = true;
            return xmlDoc;
        }
        catch (System.Exception exc)
        {
            Debug.Log("It seems we need to skip BOM at XML:" + textasset.name + "\n" + exc.StackTrace);
            parseOK = false;
        }
        //if 1. fail, skip BOM, and parse again.
        if (parseOK == false)
        {
            System.IO.StringReader stringReader = new System.IO.StringReader(textasset.text);
            stringReader.Read(); // skip BOM
            System.Xml.XmlReader reader = System.Xml.XmlReader.Create(stringReader);
            xmlDoc.Load(reader);
            reader.Close();
            stringReader.Close();
        }
        return xmlDoc;
    }

    /// <summary>
    /// Parses the character localization XML file.
    /// </summary>
    static IDictionary<string, LocalizeCharacter> ParseCharacterLocalizationXMLFile(XmlDocument xmlDoc, SystemLanguage language)
    {
        IDictionary<string, LocalizeCharacter> ret = new Dictionary<string, LocalizeCharacter>();
        XmlElement root = xmlDoc.DocumentElement;
        XmlNodeList xmlNodeList = root.GetElementsByTagName("character");
        foreach (XmlNode node in xmlNodeList)
        {
            XmlElement characterElement = (XmlElement)node;
            LocalizeCharacter localization_Character = new LocalizeCharacter();
            localization_Character.CharacterID = characterElement.GetAttribute("id"); ;
            localization_Character.CharacterName = characterElement.GetAttribute(language.ToString());
            ///CharacterIconPath can be empty
            localization_Character.CharacterIconPath = characterElement.HasAttribute("imagepath") ? characterElement.GetAttribute("imagepath") : string.Empty;
            ///the image can be null
            localization_Character.PortraitIconTexture = localization_Character.CharacterIconPath != string.Empty ? (Texture2D)Resources.Load(localization_Character.CharacterIconPath, typeof(Texture2D)) : null;
            ret.Add(localization_Character.CharacterID, localization_Character);
        }
        return ret;
    }

    /// <summary>
    /// Parses the dialog localization XML file.
    /// </summary>
    static IDictionary<string, LocalizedDialogue> ParseDialogLocalizationXMLFile(XmlDocument xmlDoc)
    {
        IDictionary<string, LocalizedDialogue> ret = new Dictionary<string, LocalizedDialogue>();
        XmlElement root = xmlDoc.DocumentElement;
        XmlNodeList xmlNodeList = root.GetElementsByTagName("dialog");
        foreach (XmlNode dialogNode in xmlNodeList)
        {
            XmlElement dialogElement = (XmlElement)dialogNode;
            LocalizedDialogue localizedDialogue = new LocalizedDialogue();
            localizedDialogue.DialogID = dialogElement.GetAttribute("id");
            foreach (XmlNode dialogItemNode in dialogElement.ChildNodes)
            {
                XmlElement dialogItemElement = (XmlElement)dialogItemNode;
                LocalizedDialogueItem localizedDialogueItem = new LocalizedDialogueItem();
                localizedDialogueItem.DialogCharacterID = dialogItemElement.GetAttribute("character");
				localizedDialogueItem.DialogImageName = dialogItemElement.GetAttribute("image");
                if (dialogItemElement.HasAttribute("pend"))
                {
                    localizedDialogueItem.pendAfterFinished = float.Parse(dialogItemElement.GetAttribute("pend"));
                }

                localizedDialogue.dialogueItem.Add(localizedDialogueItem);
            }
            ret.Add(localizedDialogue.DialogID, localizedDialogue);
        }
        return ret;
    }

    public static LocalizedDialogue GetDialogue(string DialogID)
    {
        return DialogueDict[DialogID];
    }

    public static LocalizeCharacter GetCharacter(string CharacterID)
    {
        return CharacterDict[CharacterID];
    }
}
