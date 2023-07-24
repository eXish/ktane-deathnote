using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;
using System;

public class DeathNoteScript : MonoBehaviour {

    public KMAudio audio;
    public KMBombInfo bomb;

    public KMSelectable[] buttons;
    public KMSelectable[] letsandsymbs;
    public GameObject[] bookRots;
    public Transform[] pages;
    public TextMesh stageText;
    public TextMesh timerText;
    public TextMesh pageeightbase;
    public Transform pageeightparent;
    public AudioClip[] deafnews;
    public AudioClip[] newsnames;
    public AudioClip[] aerokira;

    public Material[] objMats;
    public Material[] copyMats;
    public Material black;

    private List<TextMesh> pageeighttexts = new List<TextMesh>();
    private List<string> previousCriminals = new List<string>();
    private string[] possiblecriminals = new string[] { "Barry Wendelson", "Kim Grefflen", "Timothy Terrence", "Raye Penber", "Harold Yugozaki", "Jule Fredrich", "Geraldine Venere", "Yuna Wattari", "Victor Palatine", "Cody Hammerton", "Ricky Jones", "Juliette Gabstine", "Zaku Hamachi", "Yvedear Balasuman", "Heinz Doof", "Burry Wendelson", "Perry Wondelson", "Kim Gremlin", "Kimberly Grefflin", "Tim Terrace", "Timothy Terronce", "Raye Penbar", "Raye Benber", "Harold Yugazaku", "Harol Yugozuki", "Julie Freedrich", "Gel Frodrich", "Gerald Vigenere", "Gareldine Venere", "Luna Wattaru", "Yuna Watterli", "Victor Palpatine", "Vector Palatine", "Cody Hampsterton", "Curdy Hamptonson", "Rick Joney", "Ricky Jones", "Juliette Gebstime", "Giliette Jabstine", "Zeku Hatachi", "Zoku Humachi", "Yvedore Belasuman", "Yvedear Balasuperman", "Heinz Doofenshmitz", "High Doof", "Brianne Bailey", "Terry Petershmit", "Gabriel Freeze", "Tao Lee", "Andy Benjing", "Andrew Chalice", "Charlie Mitch", "June Herring", "Gabby Shrihaka", "Justin Ma", "Kylie Deterny", "Xia Kenshi", "Tony Peterson", "Carly Heavens", "Benjamin Junebug", "Yukasa Wenshi", "Quinton Benter", "Ray Herding", "Brandon Kang", "River Jones", "Emily Holiday", "Jessica Neddy", "Anthony Strong", "Henry Long", "Oscar Higgins", "Lenard Greobrant", "Freddy Hertz" };
    private List<string> criminals = new List<string>();
    private List<string> typeofcrime = new List<string>();
    private string[] allCauses = new string[] { "Heart Attack", "Electric Chair", "Drowned in Acid", "Crushed by Boulder", "Trip on Banana Peel", "Choked by Old Man", "Deadly Disease", "Shot Through the Heart", "Hit by Car", "Bomb Explosion" };
    // -1 = any can be used, 0 = boulder and hit by car, 1 = acid, banana, and heart attack, 2 = chair, disease, and explosion, 3 = choked by old man and shot through heart
    private List<int> correctCause = new List<int>();
    private List<bool> die = new List<bool>();
    // 0 = NOTHING, 1 = 4-12 hrs, 2 = 14-20 hrs, 3 = 31-58 mins, 4 = 0-45 mins, 5 = prime secs
    private List<int> timedie = new List<int>();
    // 0 = RETURN, 1 = PASS ON, 2 = GO ON, 3 = RETURN after the day's deletions, 4 = PASS ON after the day's deletions
    private List<int> newsPlaylistIndexes = new List<int>();
    private List<int> newsMajorNamesIndexes = new List<int>();
    private List<int> newsPettyNamesIndexes = new List<int>();
    private List<int> kiraInstructionsIndexes = new List<int>();
    private KeyCode[] keyboardKeys = { KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X, KeyCode.Y, KeyCode.Z, KeyCode.Alpha0, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Space, KeyCode.Semicolon, KeyCode.Backspace, KeyCode.Return };
    private int buttonToPress = -1;
    private int pageeightindex = 0;
    private bool focused = false;
    private bool open = false;
    private bool animating = false;
    private bool cooldown = true;
    private bool alreadyAsked = false;
    private int hrs, mins, secs = 0;
    private int stage = -1;

    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake()
    {
        pageeighttexts.Add(pageeightbase);
        moduleId = moduleIdCounter++;
        foreach (KMSelectable obj in buttons)
        {
            KMSelectable pressed = obj;
            pressed.OnInteract += delegate () { PressButton(pressed); return false; };
        }
        foreach (KMSelectable obj in letsandsymbs)
        {
            KMSelectable pressed = obj;
            pressed.OnInteract += delegate () { PressLetterOrSymbol(pressed); return false; };
        }
        GetComponent<KMSelectable>().OnFocus += delegate () { focused = true; };
        GetComponent<KMSelectable>().OnDefocus += delegate () { focused = false; };
        GetComponent<KMBombModule>().OnActivate += OnActivate;
    }

    void Start () {
        alreadyAsked = false;
        stage++;
        Debug.LogFormat("[Death Note #{0}] It is currently Day {1} of your reign as Kira.", moduleId, stage+1);
        hrs = UnityEngine.Random.Range(0, 24);
        mins = UnityEngine.Random.Range(0, 60);
        secs = UnityEngine.Random.Range(0, 60);
        if (hrs == 23)
        {
            mins = UnityEngine.Random.Range(0, 31);
            if (mins == 30)
            {
                secs = 0;
            }
        }
        if (stageText.text != "")
        {
            stageText.text = "" + (stage + 1);
            string create = "";
            if (hrs < 10)
            {
                create += "0" + hrs;
            }
            else if (hrs >= 10)
            {
                create += "" + hrs;
            }
            create += ":";
            if (mins < 10)
            {
                create += "0" + mins;
            }
            else if (mins >= 10)
            {
                create += "" + mins;
            }
            create += ":";
            if (secs < 10)
            {
                create += "0" + secs;
            }
            else if (secs >= 10)
            {
                create += "" + secs;
            }
            timerText.text = create;
            StartCoroutine(timeUpdate());
        }
        Debug.LogFormat("[Death Note #{0}] The time of day is {1}:{2}:{3}.", moduleId, (hrs < 10) ? "0" + hrs : "" + hrs, (mins < 10) ? "0" + mins : "" + mins, (secs < 10) ? "0" + secs : "" + secs);
        int intro = -1;
        if (hrs >= 6 && hrs <= 11)
            intro = 1;
        else if (hrs >= 12 && hrs <= 18)
            intro = 2;
        else if (hrs >= 19)
            intro = 3;
        else
        {
            int[] choices = new int[] { 0, 4 };
            intro = choices[UnityEngine.Random.Range(0, choices.Length)];
        }
        newsPlaylistIndexes.Add(intro);
        int numcrims = UnityEngine.Random.Range(0, 9);
        for (int i = 0; i < numcrims; i++)
        {
            int pick = UnityEngine.Random.Range(0, possiblecriminals.Length);
            while (criminals.Contains(possiblecriminals[pick]) || previousCriminals.Contains(possiblecriminals[pick]))
            {
                pick = UnityEngine.Random.Range(0, possiblecriminals.Length);
            }
            int type = UnityEngine.Random.Range(0, 2);
            if (type == 0)
            {
                typeofcrime.Add("petty");
                newsPettyNamesIndexes.Add(pick);
            }
            else
            {
                typeofcrime.Add("major");
                newsMajorNamesIndexes.Add(pick);
            }
            criminals.Add(possiblecriminals[pick]);
            previousCriminals.Add(possiblecriminals[pick]);
            die.Add(false);
            timedie.Add(0);
            correctCause.Add(-1);
        }
        if (newsMajorNamesIndexes.Count() == 0 && newsPettyNamesIndexes.Count() == 0)
        {
            for (int i = 0; i < UnityEngine.Random.Range(1, 4); i++)
            {
                int index = UnityEngine.Random.Range(5, 9);
                while (newsPlaylistIndexes.Contains(index))
                {
                    index = UnityEngine.Random.Range(5, 9);
                }
                newsPlaylistIndexes.Add(index);
            }
        }
        else
        {
            List<int> temp = new List<int>();
            int start = 0;
            int end = 2;
            if (newsMajorNamesIndexes.Count() == 0 || newsPettyNamesIndexes.Count() == 0)
                end = 3;
            if (newsMajorNamesIndexes.Count() == 0 || newsPettyNamesIndexes.Count() > 1)
                start = 1;
            for (int i = 0; i < UnityEngine.Random.Range(start, end); i++)
            {
                int index = UnityEngine.Random.Range(5, 9);
                while (temp.Contains(index))
                {
                    index = UnityEngine.Random.Range(5, 9);
                }
                temp.Add(index);
            }
            if (newsMajorNamesIndexes.Count() > 1)
            {
                temp.Add(UnityEngine.Random.Range(-12, -10));
                while (temp[0] != -12 && temp[0] != -11)
                {
                    temp = temp.Shuffle();
                }
                if (newsPettyNamesIndexes.Count() > 1)
                {
                    int ind = new int[] { -5, -4, -13 }.PickRandom();
                    temp.Add(ind);
                    if (ind != -13)
                    {
                        while ((temp[1] != -5 && temp[1] != -4) || (temp[0] != -12 && temp[0] != -11))
                        {
                            temp = temp.Shuffle();
                        }
                    }
                    else
                    {
                        while ((temp[1] == -13) || (temp[0] != -12 && temp[0] != -11))
                        {
                            temp = temp.Shuffle();
                        }
                    }
                }
                else if (newsPettyNamesIndexes.Count() == 1)
                {
                    int ind = new int[] { -3, -2, -14 }.PickRandom();
                    temp.Add(ind);
                    if (ind == -3)
                    {
                        while (temp[1] != -3 || (temp[0] != -12 && temp[0] != -11))
                        {
                            temp = temp.Shuffle();
                        }
                    }
                    else
                    {
                        temp = temp.Shuffle();
                        while (temp[0] != -12 && temp[0] != -11)
                        {
                            temp = temp.Shuffle();
                        }
                    }
                }
            }
            else if (newsMajorNamesIndexes.Count() == 1)
            {
                temp.Add(UnityEngine.Random.Range(-10, -7));
                if (newsPettyNamesIndexes.Count() > 1)
                {
                    temp.Add(-13);
                    temp = temp.Shuffle();
                }
                else if (newsPettyNamesIndexes.Count() == 1)
                {
                    temp.Add(new int[] { -2, -14 }.PickRandom());
                    temp = temp.Shuffle();
                }
            }
            else if (newsMajorNamesIndexes.Count() == 0 && newsPettyNamesIndexes.Count() != 0)
            {
                temp.Add(UnityEngine.Random.Range(-7, -5));
                while (temp[0] != -7 && temp[0] != -6)
                {
                    temp = temp.Shuffle();
                }
                if (newsPettyNamesIndexes.Count() > 1)
                {
                    temp.Add(-13);
                    temp = temp.Shuffle();
                    while (temp[0] != -7 && temp[0] != -6)
                    {
                        temp = temp.Shuffle();
                    }
                }
                else if (newsPettyNamesIndexes.Count() == 1)
                {
                    temp.Add(new int[] { -2, -14 }.PickRandom());
                    temp = temp.Shuffle();
                    while (temp[0] != -7 && temp[0] != -6)
                    {
                        temp = temp.Shuffle();
                    }
                }
            }
            for (int i = 0; i < temp.Count(); i++)
            {
                newsPlaylistIndexes.Add(temp[i]);
            }
        }
        newsPlaylistIndexes.Add(UnityEngine.Random.Range(22, 25));
        string log = "";
        string log2 = "";
        for (int i = 0; i < numcrims; i++)
        {
            if (i == numcrims - 1 && numcrims != 1)
            {
                log += "and " + criminals[i];
                log2 += "and " + typeofcrime[i];
            }
            else if (i == numcrims - 1 && numcrims == 1)
            {
                log += criminals[i];
                log2 += typeofcrime[i];
            }
            else
            {
                log += criminals[i] + ", ";
                log2 += typeofcrime[i] + ", ";
            }
        }
        int[] percents = { 0, 0, 0, 10, 25, 50 };
        if (stage > 2)
        {
            if (stage == 3)
                kiraInstructionsIndexes.Add(UnityEngine.Random.Range(3, 6));
            else
                kiraInstructionsIndexes.Add(UnityEngine.Random.Range(6, 10));
            int rando = UnityEngine.Random.Range(0, 100);
            if (rando < percents[stage])
            {
                int rando2 = UnityEngine.Random.Range(0, 2);
                if (rando2 == 0)
                {
                    int rando3 = UnityEngine.Random.Range(0, 2);
                    if (rando3 == 0)
                        buttonToPress = 0;
                    else
                        buttonToPress = 3;
                }
                else
                {
                    int rando3 = UnityEngine.Random.Range(0, 2);
                    if (rando3 == 0)
                        buttonToPress = 1;
                    else
                        buttonToPress = 4;
                }
            }
            else
                kiraInstructionsIndexes.Add(UnityEngine.Random.Range(16, 18));
        }
        else
            kiraInstructionsIndexes.Add(UnityEngine.Random.Range(1, 3));
        if (buttonToPress != 0 && buttonToPress != 1)
        {
            if (buttonToPress != 3 && buttonToPress != 4)
                buttonToPress = 2;
            if (numcrims == 0)
            {
                Debug.LogFormat("[Death Note #{0}] On the news these criminals have been broadcasted: none", moduleId);
                Debug.LogFormat("[Death Note #{0}] Each criminal committed this type of crime according to the news: none", moduleId);
            }
            else
            {
                Debug.LogFormat("[Death Note #{0}] On the news these criminals have been broadcasted: {1}", moduleId, log);
                Debug.LogFormat("[Death Note #{0}] Each criminal committed this type of crime according to the news: {1}", moduleId, log2);
            }
            List<string> always = new List<string>() { "Barry Wendelson", "Kim Grefflen", "Timothy Terrence", "Raye Penber", "Harold Yugozaki", "Jule Fredrich", "Geraldine Venere", "Yuna Wattari", "Victor Palatine", "Cody Hammerton", "Ricky Jones", "Juliette Gabstine", "Zaku Hamachi", "Yvedear Balasuman", "Heinz Doof" };
            List<string> alwayshas = new List<string>();
            List<string> usedPrevious = new List<string>();
            string log3 = "";
            for (int i = 0; i < numcrims; i++)
            {
                if (always.Contains(criminals[i]))
                {
                    alwayshas.Add(criminals[i]);
                    usedPrevious.Add(criminals[i]);
                    die[i] = true;
                }
            }
            log3 = getLogMessage(alwayshas);
            if (alwayshas.Count != 0)
                Debug.LogFormat("[Death Note #{0}] The following of the broadcasted criminals must be deleted no matter what: {1}", moduleId, log3);
            List<string> rule = new List<string>();
            log3 = "";
            for (int i = 0; i < numcrims; i++)
            {
                if (criminals[i].Count(f => f == 'b' || f == 'B') == 2 && typeofcrime[i] == "petty" && !usedPrevious.Contains(criminals[i]))
                {
                    rule.Add(criminals[i]);
                    usedPrevious.Add(criminals[i]);
                    die[i] = true;
                }
            }
            log3 = getLogMessage(rule);
            if (rule.Count != 0)
                Debug.LogFormat("[Death Note #{0}] The following of the broadcasted criminals must be deleted due to having two letter B's in their name and committing a petty crime: {1}", moduleId, log3);
            rule = new List<string>();
            log3 = "";
            for (int i = 0; i < numcrims; i++)
            {
                if (criminals[i].Count(f => f == 'a' || f == 'A' || f == 'e' || f == 'E' || f == 'i' || f == 'I' || f == 'o' || f == 'O' || f == 'u' || f == 'U') >= 5 && typeofcrime[i] == "major" && (stage == 1 || stage == 3) && !usedPrevious.Contains(criminals[i]))
                {
                    rule.Add(criminals[i]);
                    usedPrevious.Add(criminals[i]);
                    die[i] = true;
                }
            }
            log3 = getLogMessage(rule);
            if (rule.Count != 0)
                Debug.LogFormat("[Death Note #{0}] The following of the broadcasted criminals must be deleted due to committing a major crime on day 2 or 4 and having at least 5 vowels (excluding y) in their name: {1}", moduleId, log3);
            rule = new List<string>();
            log3 = "";
            for (int i = 0; i < numcrims; i++)
            {
                if (criminals[i].Count(f => f == 'a' || f == 'A' || f == 'e' || f == 'E' || f == 'i' || f == 'I' || f == 'o' || f == 'O' || f == 'u' || f == 'U') >= 5 && typeofcrime[i] == "major" && (stage == 1 || stage == 3) && !usedPrevious.Contains(criminals[i]))
                {
                    rule.Add(criminals[i]);
                    usedPrevious.Add(criminals[i]);
                    die[i] = true;
                }
            }
            log3 = getLogMessage(rule);
            if (rule.Count != 0)
                Debug.LogFormat("[Death Note #{0}] The following of the broadcasted criminals must be deleted due to committing a major crime on day 2 or 4 and having at least 5 vowels (excluding y) in their name: {1}", moduleId, log3);
            rule = new List<string>();
            log3 = "";
            for (int i = 0; i < numcrims; i++)
            {
                if ((criminals[i].Contains('m') || criminals[i].Contains('M') || criminals[i].Contains('f') || criminals[i].Contains('F')) && (stage == 0 || stage == 5) && !usedPrevious.Contains(criminals[i]))
                {
                    rule.Add(criminals[i]);
                    usedPrevious.Add(criminals[i]);
                    die[i] = true;
                }
            }
            log3 = getLogMessage(rule);
            if (rule.Count != 0)
                Debug.LogFormat("[Death Note #{0}] The following of the broadcasted criminals must be deleted due to having an M or F in their name and it being day 1 or 6: {1}", moduleId, log3);
            rule = new List<string>();
            log3 = "";
            for (int i = 0; i < numcrims; i++)
            {
                if ((typeofcrime[i] == "major") && criminals[i].Split(' ')[0].Length < 5 && !usedPrevious.Contains(criminals[i]))
                {
                    rule.Add(criminals[i]);
                    usedPrevious.Add(criminals[i]);
                    die[i] = true;
                }
            }
            log3 = getLogMessage(rule);
            if (rule.Count != 0)
                Debug.LogFormat("[Death Note #{0}] The following of the broadcasted criminals must be deleted due to committing a major crime and their first name consisting of < 5 letters: {1}", moduleId, log3);
            for (int i = 0; i < numcrims; i++)
            {
                if (die[i])
                {
                    if (typeofcrime[i] == "major" && stage == 2 && !alwayshas.Contains(criminals[i]))
                    {
                        timedie[i] = 1;
                        Debug.LogFormat("[Death Note #{0}] Criminal {1} must be deleted only if the time of death is between 4 and 12 o'clock.", moduleId, criminals[i]);
                    }
                    else if (typeofcrime[i] == "petty" && (stage == 1 || stage == 4) && !alwayshas.Contains(criminals[i]))
                    {
                        timedie[i] = 2;
                        Debug.LogFormat("[Death Note #{0}] Criminal {1} must be deleted only if the time of death is between 14 and 20 o'clock.", moduleId, criminals[i]);
                    }
                    else if (typeofcrime[i] == "petty" && stage == 3 && !alwayshas.Contains(criminals[i]))
                    {
                        timedie[i] = 3;
                        Debug.LogFormat("[Death Note #{0}] Criminal {1} must be deleted only if the time of death has its minutes between 31 and 58.", moduleId, criminals[i]);
                    }
                    else if (typeofcrime[i] == "major" && stage == 0 && !alwayshas.Contains(criminals[i]))
                    {
                        timedie[i] = 4;
                        Debug.LogFormat("[Death Note #{0}] Criminal {1} must be deleted only if the time of death has its minutes between 0 and 45.", moduleId, criminals[i]);
                    }
                    else if (typeofcrime[i] == "major" && stage == 5 && !alwayshas.Contains(criminals[i]))
                    {
                        timedie[i] = 5;
                        Debug.LogFormat("[Death Note #{0}] Criminal {1} must be deleted only if the time of death has its seconds as a prime number.", moduleId, criminals[i]);
                    }
                    else
                    {
                        Debug.LogFormat("[Death Note #{0}] Criminal {1} may be deleted at any time.", moduleId, criminals[i]);
                    }
                    if (typeofcrime[i] == "petty" && (stage == 0 || stage == 3) && !alwayshas.Contains(criminals[i]))
                    {
                        correctCause[i] = 0;
                        Debug.LogFormat("[Death Note #{0}] Criminal {1} must be deleted through the cause of Crushed by Boulder or Hit by Car.", moduleId, criminals[i]);
                    }
                    else if (typeofcrime[i] == "petty" && (stage == 1 || stage == 2 || stage == 4 || stage == 5) && !alwayshas.Contains(criminals[i]))
                    {
                        correctCause[i] = 1;
                        Debug.LogFormat("[Death Note #{0}] Criminal {1} must be deleted through the cause of Drowned in Acid, Trip on Banana Peel, or Heart Attack.", moduleId, criminals[i]);
                    }
                    else if (typeofcrime[i] == "major" && (criminals[i].Split(' ')[0].Contains("A") || criminals[i].Split(' ')[0].Contains("C")) && !alwayshas.Contains(criminals[i]))
                    {
                        correctCause[i] = 2;
                        Debug.LogFormat("[Death Note #{0}] Criminal {1} must be deleted through the cause of Electric Chair, Deadly Disease, or Bomb Explosion.", moduleId, criminals[i]);
                    }
                    else if (typeofcrime[i] == "major" && criminals[i].Split(' ')[1].Last().Equals("y") && !alwayshas.Contains(criminals[i]))
                    {
                        correctCause[i] = 3;
                        Debug.LogFormat("[Death Note #{0}] Criminal {1} must be deleted through the cause of Choked by Old Man or Shot Through the Heart.", moduleId, criminals[i]);
                    }
                    else
                    {
                        Debug.LogFormat("[Death Note #{0}] Criminal {1} may be deleted through any cause.", moduleId, criminals[i]);
                    }
                }
            }
            
        }
        if (buttonToPress == 0)
        {
            Debug.LogFormat("[Death Note #{0}] The real Kira wishes for you to return the death note immediately.", moduleId);
        }
        else if (buttonToPress == 1)
        {
            Debug.LogFormat("[Death Note #{0}] The real Kira wishes for you to pass on the death note to a new Kira immediately.", moduleId);
        }
        else if (buttonToPress == 3)
        {
            kiraInstructionsIndexes.Add(UnityEngine.Random.Range(12, 14));
            Debug.LogFormat("[Death Note #{0}] After all deletable criminals have been deleted, the real Kira wishes for you to return the death note.", moduleId);
        }
        else if (buttonToPress == 4)
        {
            kiraInstructionsIndexes.Add(UnityEngine.Random.Range(14, 16));
            Debug.LogFormat("[Death Note #{0}] After all deletable criminals have been deleted, the real Kira wishes for you to pass on the death note to a new Kira.", moduleId);
        }
        else
        {
            Debug.LogFormat("[Death Note #{0}] After all deletable criminals have been deleted, you must go on with your day.", moduleId);
        }
    }

    void OnActivate()
    {
        stageText.text = ""+1;
        StartCoroutine(timeUpdate());
        cooldown = false;
    }

    void Update()
    {
        if (focused || Application.isEditor)
        {
            for (int i = 0; i < keyboardKeys.Length; i++)
            {
                if (Input.GetKeyDown(keyboardKeys[i]))
                    letsandsymbs[i].OnInteract();
            }
        }
    }

    void PressButton(KMSelectable pressed)
    {
        if (moduleSolved != true && !animating && !cooldown)
        {
            if (pressed == buttons[0] && !open)
            {
                open = true;
                StartCoroutine(openBook());
            }
            else if (pressed == buttons[1] && open)
            {
                open = false;
                StartCoroutine(closeBook());
            }
            else if (pressed == buttons[2])
            {
                pressed.AddInteractionPunch();
                audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, pressed.transform);
                StartCoroutine(Aero(alreadyAsked));
                if (alreadyAsked)
                {
                    kiraInstructionsIndexes.Clear();
                    kiraInstructionsIndexes.Add(UnityEngine.Random.Range(10, 12));
                }
                else
                    alreadyAsked = true;
            }
            else if (pressed == buttons[3])
            {
                pressed.AddInteractionPunch();
                audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, pressed.transform);
                StartCoroutine(Deaf());
            }
            else if (pressed == buttons[4] && !open)
            {
                pressed.AddInteractionPunch();
                audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, pressed.transform);
                if (buttonToPress != 2)
                {
                    Debug.LogFormat("[Death Note #{0}] You decided to go on with your day, which is not what you should have done. You will suffer a cruel death... Strike! Module resetting...", moduleId);
                    GetComponent<KMBombModule>().HandleStrike();
                    previousCriminals.Clear();
                    stage = -1;
                    reset();
                    return;
                }
                else if (buttonToPress == 2)
                {
                    Debug.LogFormat("[Death Note #{0}] You decided to go on with your day, which is what you should have done.", moduleId);
                }
                if (die.Count(f => f == true) != 0)
                {
                    doChecks(0);
                }
                else if (die.Count(f => f == true) == 0 && (pageeighttexts.Count != 1 || pageeightbase.text.Length != 0))
                {
                    Debug.LogFormat("[Death Note #{0}] You did not leave the death note empty and there were supposed to be no deletions today. You will suffer a cruel death... Strike! Module resetting...", moduleId);
                    GetComponent<KMBombModule>().HandleStrike();
                    previousCriminals.Clear();
                    stage = -1;
                    reset();
                }
                else
                {
                    if (stage == 5)
                    {
                        Debug.LogFormat("[Death Note #{0}] You left the death note empty and there were supposed to be no deletions today. Module solved!", moduleId);
                        audio.PlaySoundAtTransform("solve", transform);
                        moduleSolved = true;
                        StopAllCoroutines();
                        GetComponent<KMBombModule>().HandlePass();
                    }
                    else
                    {
                        Debug.LogFormat("[Death Note #{0}] You left the death note empty and there were supposed to be no deletions today. You live to see another day... Advancing to day {1}...", moduleId, stage + 2);
                        reset();
                    }
                }
            }
            else if (pressed == buttons[5] && !open)
            {
                pressed.AddInteractionPunch();
                audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, pressed.transform);
                if (buttonToPress != 0 && buttonToPress != 3)
                {
                    Debug.LogFormat("[Death Note #{0}] You decided to return the death note to the real Kira, which is not what you should have done. You will suffer a cruel death... Strike! Module resetting...", moduleId);
                    GetComponent<KMBombModule>().HandleStrike();
                    previousCriminals.Clear();
                    stage = -1;
                    reset();
                    return;
                }
                else if (buttonToPress == 0)
                {
                    Debug.LogFormat("[Death Note #{0}] You decided to return the death note to the real Kira, which is what you should have done.", moduleId);
                    if (pageeighttexts.Count != 1 || pageeightbase.text.Length != 0)
                    {
                        Debug.LogFormat("[Death Note #{0}] You did not leave the death note empty before returning the death note to the real Kira. You will suffer a cruel death... Strike! Module resetting...", moduleId);
                        GetComponent<KMBombModule>().HandleStrike();
                        previousCriminals.Clear();
                        stage = -1;
                        reset();
                    }
                    else
                    {
                        Debug.LogFormat("[Death Note #{0}] You left the death note empty before returning the death note to the real Kira. Module solved!", moduleId);
                        audio.PlaySoundAtTransform("solve", transform);
                        moduleSolved = true;
                        StopAllCoroutines();
                        GetComponent<KMBombModule>().HandlePass();
                    }
                }
                else if (buttonToPress == 3)
                {
                    Debug.LogFormat("[Death Note #{0}] You decided to return the death note to the real Kira, which is what you should have done.", moduleId);
                    if (die.Count(f => f == true) != 0)
                    {
                        doChecks(1);
                    }
                    else if (die.Count(f => f == true) == 0 && (pageeighttexts.Count != 1 || pageeightbase.text.Length != 0))
                    {
                        Debug.LogFormat("[Death Note #{0}] You did not leave the death note empty and there were supposed to be no deletions today before returning the death note to the real Kira. You will suffer a cruel death... Strike! Module resetting...", moduleId);
                        GetComponent<KMBombModule>().HandleStrike();
                        previousCriminals.Clear();
                        stage = -1;
                        reset();
                    }
                    else
                    {
                        Debug.LogFormat("[Death Note #{0}] You left the death note empty before returning the death note to the real Kira. Module solved!", moduleId);
                        audio.PlaySoundAtTransform("solve", transform);
                        moduleSolved = true;
                        StopAllCoroutines();
                        GetComponent<KMBombModule>().HandlePass();
                    }
                }
            }
            else if (pressed == buttons[6] && !open)
            {
                pressed.AddInteractionPunch();
                audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, pressed.transform);
                if (buttonToPress != 1 && buttonToPress != 4)
                {
                    Debug.LogFormat("[Death Note #{0}] You decided to mail the death note to a new Kira, which is not what you should have done. You will suffer a cruel death... Strike! Module resetting...", moduleId);
                    GetComponent<KMBombModule>().HandleStrike();
                    previousCriminals.Clear();
                    stage = -1;
                    reset();
                    return;
                }
                else if (buttonToPress == 1)
                {
                    Debug.LogFormat("[Death Note #{0}] You decided to mail the death note to a new Kira, which is what you should have done.", moduleId);
                    if (pageeighttexts.Count != 1 || pageeightbase.text.Length != 0)
                    {
                        Debug.LogFormat("[Death Note #{0}] You did not leave the death note empty before mailing the death note to a new Kira. You will suffer a cruel death... Strike! Module resetting...", moduleId);
                        GetComponent<KMBombModule>().HandleStrike();
                        previousCriminals.Clear();
                        stage = -1;
                        reset();
                    }
                    else
                    {
                        Debug.LogFormat("[Death Note #{0}] You left the death note empty before mailing the death note to a new Kira. Module solved!", moduleId);
                        audio.PlaySoundAtTransform("solve", transform);
                        moduleSolved = true;
                        StopAllCoroutines();
                        GetComponent<KMBombModule>().HandlePass();
                    }
                }
                else if (buttonToPress == 4)
                {
                    Debug.LogFormat("[Death Note #{0}] You decided to mail the death note to a new Kira, which is what you should have done.", moduleId);
                    if (die.Count(f => f == true) != 0)
                    {
                        doChecks(2);
                    }
                    else if (die.Count(f => f == true) == 0 && (pageeighttexts.Count != 1 || pageeightbase.text.Length != 0))
                    {
                        Debug.LogFormat("[Death Note #{0}] You did not leave the death note empty and there were supposed to be no deletions today before mailing the death note to a new Kira. You will suffer a cruel death... Strike! Module resetting...", moduleId);
                        GetComponent<KMBombModule>().HandleStrike();
                        previousCriminals.Clear();
                        stage = -1;
                        reset();
                    }
                    else
                    {
                        Debug.LogFormat("[Death Note #{0}] You left the death note empty before mailing the death note to a new Kira. Module solved!", moduleId);
                        audio.PlaySoundAtTransform("solve", transform);
                        moduleSolved = true;
                        StopAllCoroutines();
                        GetComponent<KMBombModule>().HandlePass();
                    }
                }
            }
        }
    }

    void PressLetterOrSymbol(KMSelectable pressed)
    {
        if (moduleSolved != true && !animating && open && !cooldown)
        {
            if (pressed == letsandsymbs[36])
            {
                pageeighttexts[pageeightindex].text += " ";
            }
            else if (pressed == letsandsymbs[38] && pageeighttexts[pageeightindex].text != "")
            {
                if (pageeighttexts[pageeightindex].text.Substring(pageeighttexts[pageeightindex].text.Length-1, 1) != " ")
                    audio.PlaySoundAtTransform("erase", pageeightparent);
                pageeighttexts[pageeightindex].text = pageeighttexts[pageeightindex].text.Substring(0, pageeighttexts[pageeightindex].text.Length-1);
            }
            else if (pressed == letsandsymbs[39] && pageeighttexts.Count < 31)
            {
                GameObject temp = Instantiate(pageeightbase.gameObject, pageeightbase.gameObject.transform.position, pageeightbase.gameObject.transform.rotation, pageeightparent);
                pageeighttexts.Add(temp.GetComponent<TextMesh>());
                pageeightindex++;
                pageeighttexts[pageeightindex].text = "";
                temp.transform.localPosition = new Vector3(3.5f, 0.0001f, -3.95f + (0.28f * (pageeighttexts.Count()-1)));
            }
            else if (pressed != letsandsymbs[38] && pageeighttexts[pageeightindex].text.Length < 49)
            {
                audio.PlaySoundAtTransform("write", pageeightparent);
                pageeighttexts[pageeightindex].text += pressed.gameObject.GetComponentInChildren<TextMesh>().text;
            }
        }
    }

    private void doChecks(int type)
    {
        // Get rid of time based criminals whose times have already passed
        for (int i = 0; i < die.Count; i++)
        {
            if (timedie[i] != 0)
            {
                if (timedie[i] == 1 && (hrs > 12))
                {
                    die[i] = false;
                    timedie[i] = 0;
                }
                else if (timedie[i] == 2 && (hrs > 20))
                {
                    die[i] = false;
                    timedie[i] = 0;
                }
                else if (timedie[i] == 3 && (hrs == 23 && mins > 58))
                {
                    die[i] = false;
                    timedie[i] = 0;
                }
                else if (timedie[i] == 4 && (hrs == 23 && mins > 45))
                {
                    die[i] = false;
                    timedie[i] = 0;
                }
            }
        }
        for (int i = 0; i < pageeighttexts.Count; i++)
        {
            if (pageeighttexts[i].text.Split(' ').Length != 5 && pageeighttexts[i].text.Split(' ').Length != 6 && pageeighttexts[i].text.Split(' ').Length != 7)
            {
                Debug.LogFormat("[Death Note #{0}] You wrote a line in the death note (line {1}) that does not match the \"[Name] [Time of Death (hrs:mins:secs)] [Cause of Death]\" format! You will suffer a cruel death... Strike! Module resetting...", moduleId, i + 1);
                GetComponent<KMBombModule>().HandleStrike();
                previousCriminals.Clear();
                stage = -1;
                reset();
                return;
            }
            string[] line = pageeighttexts[i].text.Split(' ');
            string name = line[0].First() + line[0].Substring(1, line[0].Length - 1).ToLower() + " " + line[1].First() + line[1].Substring(1, line[1].Length - 1).ToLower();
            if (!criminals.Contains(name))
            {
                Debug.LogFormat("[Death Note #{0}] You wrote down the name '{1}' which did not appear in the broadcast! You will suffer a cruel death... Strike! Module resetting...", moduleId, name);
                GetComponent<KMBombModule>().HandleStrike();
                previousCriminals.Clear();
                stage = -1;
                reset();
                return;
            }
            else if (!die[criminals.IndexOf(name)])
            {
                Debug.LogFormat("[Death Note #{0}] You wrote down the name '{1}' who is a criminal but cannot be deleted today! You will suffer a cruel death... Strike! Module resetting...", moduleId, name);
                GetComponent<KMBombModule>().HandleStrike();
                previousCriminals.Clear();
                stage = -1;
                reset();
                return;
            }
            if (line[2].Length != 8)
            {
                Debug.LogFormat("[Death Note #{0}] You wrote down the time of death for '{1}' to be '{2}', which is not a valid hrs:mins:secs format! You will suffer a cruel death... Strike! Module resetting...", moduleId, name, line[2]);
                GetComponent<KMBombModule>().HandleStrike();
                previousCriminals.Clear();
                stage = -1;
                reset();
                return;
            }
            string[] times = line[2].Split(':');
            if (times.Length != 3)
            {
                Debug.LogFormat("[Death Note #{0}] You wrote down the time of death for '{1}' to be '{2}', which is not a valid hrs:mins:secs format! You will suffer a cruel death... Strike! Module resetting...", moduleId, name, line[2]);
                GetComponent<KMBombModule>().HandleStrike();
                previousCriminals.Clear();
                stage = -1;
                reset();
                return;
            }
            int temphrs, tempmins, tempsecs = 0;
            if (!int.TryParse(times[0], out temphrs) || !int.TryParse(times[1], out tempmins) || !int.TryParse(times[2], out tempsecs))
            {
                Debug.LogFormat("[Death Note #{0}] You wrote down the time of death for '{1}' to be '{2}', but the hours, minutes, or seconds are not numbers! You will suffer a cruel death... Strike! Module resetting...", moduleId, name, line[2]);
                GetComponent<KMBombModule>().HandleStrike();
                previousCriminals.Clear();
                stage = -1;
                reset();
                return;
            }
            if (temphrs > 23 || tempmins > 59 || tempsecs > 59)
            {
                Debug.LogFormat("[Death Note #{0}] You wrote down the time of death for '{1}' to be '{2}', but the written time is out of range of 00:00:00 and 23:59:59! You will suffer a cruel death... Strike! Module resetting...", moduleId, name, line[2]);
                GetComponent<KMBombModule>().HandleStrike();
                previousCriminals.Clear();
                stage = -1;
                reset();
                return;
            }
            if ((hrs * 3600 + mins * 60 + secs) > (temphrs * 3600 + tempmins * 60 + tempsecs))
            {
                Debug.LogFormat("[Death Note #{0}] You wrote down the time of death for '{1}' to be '{2}', but this time has already passed today! You will suffer a cruel death... Strike! Module resetting...", moduleId, name, line[2]);
                GetComponent<KMBombModule>().HandleStrike();
                previousCriminals.Clear();
                stage = -1;
                reset();
                return;
            }
            if (timedie[criminals.IndexOf(name)] != 0)
            {
                if ((timedie[criminals.IndexOf(name)] == 1 && (temphrs > 12 || temphrs < 4)) || (timedie[criminals.IndexOf(name)] == 2 && (temphrs > 20 || temphrs < 14)) || (timedie[criminals.IndexOf(name)] == 3 && (tempmins > 58 || tempmins < 31)) || (timedie[criminals.IndexOf(name)] == 4 && (tempmins > 45)) || (timedie[criminals.IndexOf(name)] == 5 && !isPrime(tempsecs)))
                {
                    Debug.LogFormat("[Death Note #{0}] You wrote down the time of death for '{1}' to be '{2}', but due to this criminal having time restrictions this cannot happen! You will suffer a cruel death... Strike! Module resetting...", moduleId, name, line[2]);
                    GetComponent<KMBombModule>().HandleStrike();
                    previousCriminals.Clear();
                    stage = -1;
                    reset();
                    return;
                }
            }
            string cause = "";
            for (int j = 3; j < line.Length; j++)
            {
                if (j == line.Length - 1)
                    cause += line[j];
                else
                    cause += line[j] + " ";
            }
            if (!validCause(cause))
            {
                Debug.LogFormat("[Death Note #{0}] You wrote down the cause of death for '{1}' to be '{2}', but this is not in the list of all causes of death! You will suffer a cruel death... Strike! Module resetting...", moduleId, name, cause);
                GetComponent<KMBombModule>().HandleStrike();
                previousCriminals.Clear();
                stage = -1;
                reset();
                return;
            }
            if (correctCause[criminals.IndexOf(name)] == 0 && (cause != "CRUSHED BY BOULDER" && cause != "HIT BY CAR") || correctCause[criminals.IndexOf(name)] == 1 && (cause != "DROWNED IN ACID" && cause != "TRIP ON BANANA PEEL" && cause != "HEART ATTACK") || correctCause[criminals.IndexOf(name)] == 2 && (cause != "ELECTRIC CHAIR" && cause != "DEADLY DISEASE" && cause != "BOMB EXPLOSION") || correctCause[criminals.IndexOf(name)] == 3 && (cause != "CHOKED BY OLD MAN" && cause != "SHOT THROUGH THE HEART"))
            {
                Debug.LogFormat("[Death Note #{0}] You wrote down the cause of death for '{1}' to be '{2}', but due to this criminal having cause of death restrictions this cannot happen! You will suffer a cruel death... Strike! Module resetting...", moduleId, name, cause);
                GetComponent<KMBombModule>().HandleStrike();
                previousCriminals.Clear();
                stage = -1;
                reset();
                return;
            }
            die[criminals.IndexOf(name)] = false;
        }
        if (die.Count(f => f == true) == 0)
        {
            if (type == 0)
            {
                if (stage == 5)
                {
                    Debug.LogFormat("[Death Note #{0}] You deleted all necessary criminals correctly for today before going on with your day for one last time. Module solved!", moduleId);
                    audio.PlaySoundAtTransform("solve", transform);
                    moduleSolved = true;
                    StopAllCoroutines();
                    GetComponent<KMBombModule>().HandlePass();
                }
                else
                {
                    Debug.LogFormat("[Death Note #{0}] You deleted all necessary criminals correctly for today. You live to see another day... Advancing to day {1}...", moduleId, stage + 2);
                    reset();
                }
            }
            else if (type == 1)
            {
                Debug.LogFormat("[Death Note #{0}] You deleted all necessary criminals correctly for today before returning the death note to the real Kira. Module solved!", moduleId);
                audio.PlaySoundAtTransform("solve", transform);
                moduleSolved = true;
                StopAllCoroutines();
                GetComponent<KMBombModule>().HandlePass();
            }
            else if (type == 2)
            {
                Debug.LogFormat("[Death Note #{0}] You deleted all necessary criminals correctly for today before mailing the death note to a new Kira. Module solved!", moduleId);
                audio.PlaySoundAtTransform("solve", transform);
                moduleSolved = true;
                StopAllCoroutines();
                GetComponent<KMBombModule>().HandlePass();
            }
        }
        else
        {
            Debug.LogFormat("[Death Note #{0}] Not all necessary criminals were deleted! You will suffer a cruel death... Strike! Module resetting...", moduleId);
            GetComponent<KMBombModule>().HandleStrike();
            previousCriminals.Clear();
            stage = -1;
            reset();
            return;
        }
    }

    private string getLogMessage(List<string> contents)
    {
        string log = "";
        for (int i = 0; i < contents.Count; i++)
        {
            if (i == contents.Count - 1 && contents.Count != 1)
            {
                log += "and " + contents[i];
            }
            else if (i == contents.Count - 1 && contents.Count == 1)
            {
                log += contents[i];
            }
            else
            {
                log += contents[i] + ", ";
            }
        }
        return log;
    }

    private bool isPrime(int number)
    {
        if (number <= 1) return false;
        if (number == 2) return true;
        if (number % 2 == 0) return false;

        var boundary = (int)Math.Floor(Math.Sqrt(number));

        for (int i = 3; i <= boundary; i += 2)
            if (number % i == 0)
                return false;

        return true;
    }

    private bool validCause(string c)
    {
        for (int i = 0; i < allCauses.Count(); i++)
        {
            if (allCauses[i].ToUpper() == c.ToUpper())
            {
                return true;
            }
        }
        return false;
    }

    private string getTime(int type)
    {
        string create = "";
        int temphrs = UnityEngine.Random.Range(0, 24);
        int tempmins = UnityEngine.Random.Range(0, 60);
        int tempsecs = UnityEngine.Random.Range(0, 60);
        if (type == 0)
        {
            while ((3600 * temphrs + 60 * tempmins + tempsecs) < (3600 * hrs + 60 * mins + secs))
            {
                temphrs = UnityEngine.Random.Range(0, 24);
                tempmins = UnityEngine.Random.Range(0, 60);
                tempsecs = UnityEngine.Random.Range(0, 60);
            }
        }
        else if (type == 1)
        {
            while ((3600 * temphrs + 60 * tempmins + tempsecs) < (3600 * hrs + 60 * mins + secs) || temphrs < 4 || temphrs > 12)
            {
                temphrs = UnityEngine.Random.Range(0, 24);
                tempmins = UnityEngine.Random.Range(0, 60);
                tempsecs = UnityEngine.Random.Range(0, 60);
            }
        }
        else if (type == 2)
        {
            while ((3600 * temphrs + 60 * tempmins + tempsecs) < (3600 * hrs + 60 * mins + secs) || temphrs < 14 || temphrs > 20)
            {
                temphrs = UnityEngine.Random.Range(0, 24);
                tempmins = UnityEngine.Random.Range(0, 60);
                tempsecs = UnityEngine.Random.Range(0, 60);
            }
        }
        else if (type == 3)
        {
            while ((3600 * temphrs + 60 * tempmins + tempsecs) < (3600 * hrs + 60 * mins + secs) || tempmins < 31 || tempmins > 58)
            {
                temphrs = UnityEngine.Random.Range(0, 24);
                tempmins = UnityEngine.Random.Range(0, 60);
                tempsecs = UnityEngine.Random.Range(0, 60);
            }
        }
        else if (type == 4)
        {
            while ((3600 * temphrs + 60 * tempmins + tempsecs) < (3600 * hrs + 60 * mins + secs) || tempmins > 45)
            {
                temphrs = UnityEngine.Random.Range(0, 24);
                tempmins = UnityEngine.Random.Range(0, 60);
                tempsecs = UnityEngine.Random.Range(0, 60);
            }
        }
        else if (type == 5)
        {
            while ((3600 * temphrs + 60 * tempmins + tempsecs) < (3600 * hrs + 60 * mins + secs) || !isPrime(tempsecs))
            {
                temphrs = UnityEngine.Random.Range(0, 24);
                tempmins = UnityEngine.Random.Range(0, 60);
                tempsecs = UnityEngine.Random.Range(0, 60);
            }
        }
        if (temphrs < 10)
        {
            create += "0" + temphrs;
        }
        else if (temphrs >= 10)
        {
            create += "" + temphrs;
        }
        create += ":";
        if (tempmins < 10)
        {
            create += "0" + tempmins;
        }
        else if (tempmins >= 10)
        {
            create += "" + tempmins;
        }
        create += ":";
        if (tempsecs < 10)
        {
            create += "0" + tempsecs;
        }
        else if (tempsecs >= 10)
        {
            create += "" + tempsecs;
        }
        return create;
    }

    private void reset()
    {
        StopAllCoroutines();
        for (int i = 1; i < pageeighttexts.Count; i++)
        {
            Destroy(pageeighttexts[i].gameObject);
        }
        pageeighttexts.Clear();
        pageeighttexts.Add(pageeightbase);
        pageeightindex = 0;
        pageeightbase.text = "";
        buttonToPress = -1;
        criminals.Clear();
        typeofcrime.Clear();
        die.Clear();
        timedie.Clear();
        correctCause.Clear();
        newsMajorNamesIndexes.Clear();
        newsPettyNamesIndexes.Clear();
        newsPlaylistIndexes.Clear();
        kiraInstructionsIndexes.Clear();
        Start();
    }

    private IEnumerator openBook()
    {
        animating = true;
        for (int i = 0; i < bookRots.Length; i++)
        {
            StartCoroutine(openPage(i));
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator openPage(int n)
    {
        audio.PlaySoundAtTransform("turnPage", pages[n]);
        int rotation = 0;
        while (rotation != 60)
        {
            yield return null;
            bookRots[n].transform.Rotate(0.0f, 0.0f, 3f, Space.Self);
            rotation++;
        }
        if (n == 7)
            animating = false;
    }

    private IEnumerator closeBook()
    {
        animating = true;
        for (int i = bookRots.Length-1; i >= 0; i--)
        {
            StartCoroutine(closePage(i));
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator closePage(int n)
    {
        audio.PlaySoundAtTransform("turnPage", pages[n]);
        int rotation = 0;
        while (rotation != 60)
        {
            yield return null;
            bookRots[n].transform.Rotate(0.0f, 0.0f, -3f, Space.Self);
            rotation++;
        }
        if (n == 0)
            animating = false;
    }

    private IEnumerator Aero(bool askStrike)
    {
        audio.PlaySoundAtTransform(aerokira[0].name, transform);
        yield return new WaitForSeconds(aerokira[0].length);
        for (int i = 0; i < kiraInstructionsIndexes.Count; i++)
        {
            audio.PlaySoundAtTransform(aerokira[kiraInstructionsIndexes[i]].name, transform);
            yield return new WaitForSeconds(aerokira[kiraInstructionsIndexes[i]].length);
        }
        if (askStrike)
        {
            Debug.LogFormat("[Death Note #{0}] Kira does not like to repeat instructions! You will suffer a cruel death... Strike! Module resetting...", moduleId);
            GetComponent<KMBombModule>().HandleStrike();
            previousCriminals.Clear();
            stage = -1;
            reset();
        }
        else if (stage < 3)
        {
            Debug.LogFormat("[Death Note #{0}] You are risking Kira getting caught by calling now! You will suffer a cruel death... Strike! Module resetting...", moduleId);
            GetComponent<KMBombModule>().HandleStrike();
            previousCriminals.Clear();
            stage = -1;
            reset();
        }
    }

    private IEnumerator Deaf()
    {
        for (int i = 0; i < newsPlaylistIndexes.Count(); i++)
        {
            if (newsPlaylistIndexes[i] > -13 && newsPlaylistIndexes[i] < -5)
            {
                bool needed = true;
                switch (newsPlaylistIndexes[i])
                {
                    case -12:
                        audio.PlaySoundAtTransform(deafnews[11].name, transform);
                        yield return new WaitForSeconds(deafnews[11].length + 0.2f);
                        break;
                    case -11:
                        audio.PlaySoundAtTransform(deafnews[12].name, transform);
                        yield return new WaitForSeconds(deafnews[12].length + 0.2f);
                        break;
                    case -10:
                        audio.PlaySoundAtTransform(deafnews[13].name, transform);
                        yield return new WaitForSeconds(deafnews[13].length + 0.2f);
                        break;
                    case -9:
                        audio.PlaySoundAtTransform(deafnews[14].name, transform);
                        yield return new WaitForSeconds(deafnews[14].length);
                        int ind = 0;
                        for (int k = 0; k < newsnames.Length; k++)
                        {
                            if (newsnames[k].name == possiblecriminals[newsMajorNamesIndexes[0]].Replace(" ", ""))
                            {
                                ind = k;
                                break;
                            }
                        }
                        audio.PlaySoundAtTransform(newsnames[ind].name, transform);
                        yield return new WaitForSeconds(newsnames[ind].length);
                        audio.PlaySoundAtTransform(deafnews[15].name, transform);
                        yield return new WaitForSeconds(deafnews[15].length + 0.2f);
                        needed = false;
                        break;
                    case -8:
                        int ind2 = 0;
                        for (int k = 0; k < newsnames.Length; k++)
                        {
                            if (newsnames[k].name == possiblecriminals[newsMajorNamesIndexes[0]].Replace(" ", ""))
                            {
                                ind2 = k;
                                break;
                            }
                        }
                        audio.PlaySoundAtTransform(newsnames[ind2].name, transform);
                        yield return new WaitForSeconds(newsnames[ind2].length);
                        audio.PlaySoundAtTransform(deafnews[16].name, transform);
                        yield return new WaitForSeconds(deafnews[16].length + 0.2f);
                        needed = false;
                        break;
                    case -7:
                        audio.PlaySoundAtTransform(deafnews[9].name, transform);
                        yield return new WaitForSeconds(deafnews[9].length + 0.2f);
                        break;
                    case -6:
                        audio.PlaySoundAtTransform(deafnews[10].name, transform);
                        yield return new WaitForSeconds(deafnews[10].length + 0.2f);
                        break;
                }
                if (needed)
                {
                    for (int j = 0; j < newsMajorNamesIndexes.Count; j++)
                    {
                        int ind = 0;
                        for (int k = 0; k < newsnames.Length; k++)
                        {
                            if (newsnames[k].name == possiblecriminals[newsMajorNamesIndexes[j]].Replace(" ", ""))
                            {
                                ind = k;
                                break;
                            }
                        }
                        audio.PlaySoundAtTransform(newsnames[ind].name, transform);
                        yield return new WaitForSeconds(newsnames[ind].length + 0.2f);
                    }
                }
            }
            else if ((newsPlaylistIndexes[i] > -6 && newsPlaylistIndexes[i] < -1) || newsPlaylistIndexes[i] < -12)
            {
                bool needed = true;
                switch (newsPlaylistIndexes[i])
                {
                    case -13:
                        audio.PlaySoundAtTransform(deafnews[27].name, transform);
                        yield return new WaitForSeconds(deafnews[27].length + 0.2f);
                        break;
                    case -5:
                        audio.PlaySoundAtTransform(deafnews[20].name, transform);
                        yield return new WaitForSeconds(deafnews[20].length + 0.2f);
                        break;
                    case -4:
                        audio.PlaySoundAtTransform(deafnews[21].name, transform);
                        yield return new WaitForSeconds(deafnews[21].length + 0.2f);
                        break;
                    case -3:
                        audio.PlaySoundAtTransform(deafnews[17].name, transform);
                        yield return new WaitForSeconds(deafnews[17].length + 0.2f);
                        break;
                    case -2:
                        audio.PlaySoundAtTransform(deafnews[18].name, transform);
                        yield return new WaitForSeconds(deafnews[18].length);
                        int ind = 0;
                        for (int k = 0; k < newsnames.Length; k++)
                        {
                            if (newsnames[k].name == possiblecriminals[newsPettyNamesIndexes[0]].Replace(" ", ""))
                            {
                                ind = k;
                                break;
                            }
                        }
                        audio.PlaySoundAtTransform(newsnames[ind].name, transform);
                        yield return new WaitForSeconds(newsnames[ind].length);
                        audio.PlaySoundAtTransform(deafnews[19].name, transform);
                        yield return new WaitForSeconds(deafnews[19].length + 0.2f);
                        needed = false;
                        break;
                    case -14:
                        audio.PlaySoundAtTransform(deafnews[25].name, transform);
                        yield return new WaitForSeconds(deafnews[25].length);
                        int ind2 = 0;
                        for (int k = 0; k < newsnames.Length; k++)
                        {
                            if (newsnames[k].name == possiblecriminals[newsPettyNamesIndexes[0]].Replace(" ", ""))
                            {
                                ind2 = k;
                                break;
                            }
                        }
                        audio.PlaySoundAtTransform(newsnames[ind2].name, transform);
                        yield return new WaitForSeconds(newsnames[ind2].length);
                        audio.PlaySoundAtTransform(deafnews[26].name, transform);
                        yield return new WaitForSeconds(deafnews[26].length + 0.2f);
                        needed = false;
                        break;
                }
                if (needed)
                {
                    for (int j = 0; j < newsPettyNamesIndexes.Count; j++)
                    {
                        int ind = 0;
                        for (int k = 0; k < newsnames.Length; k++)
                        {
                            if (newsnames[k].name == possiblecriminals[newsPettyNamesIndexes[j]].Replace(" ", ""))
                            {
                                ind = k;
                                break;
                            }
                        }
                        audio.PlaySoundAtTransform(newsnames[ind].name, transform);
                        yield return new WaitForSeconds(newsnames[ind].length + 0.2f);
                    }
                }
            }
            else
            {
                if (i == 0)
                {
                    audio.PlaySoundAtTransform(deafnews[deafnews.Length-1].name, transform);
                    yield return new WaitForSeconds(deafnews[deafnews.Length - 1].length - 0.8f);
                }
                audio.PlaySoundAtTransform(deafnews[newsPlaylistIndexes[i]].name, transform);
                yield return new WaitForSeconds(deafnews[newsPlaylistIndexes[i]].length + 0.2f);
            }
        }
    }

    private IEnumerator timeUpdate()
    {
        secs++;
        if (secs > 59)
        {
            secs = 0;
            mins++;
            if (mins > 59)
            {
                mins = 0;
                hrs++;
                if (hrs > 23)
                {
                    hrs = 0;
                    Debug.LogFormat("[Death Note #{0}] You did not complete all your deletions before the next day started! You will suffer a cruel death... Strike! Module resetting...", moduleId);
                    GetComponent<KMBombModule>().HandleStrike();
                    previousCriminals.Clear();
                    stage = -1;
                    reset();
                }
            }
        }
        string create = "";
        if (hrs < 10)
        {
            create += "0" + hrs;
        }
        else if (hrs >= 10)
        {
            create += "" + hrs;
        }
        create += ":";
        if (mins < 10)
        {
            create += "0" + mins;
        }
        else if (mins >= 10)
        {
            create += "" + mins;
        }
        create += ":";
        if (secs < 10)
        {
            create += "0" + secs;
        }
        else if (secs >= 10)
        {
            create += "" + secs;
        }
        timerText.text = create;
        yield return new WaitForSeconds(1f);
        StartCoroutine(timeUpdate());
    }

    //twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} open/close [Opens or closes the death note] | !{0} write <text> [Writes out 'text' on the current line] | !{0} delete (#) [Deletes the characters on the current line (up to '#' times)] | !{0} enter/newline [Moves the current line down 1] | !{0} goon/passon/return/call/newspaper [Presses the specified button]";
    #pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        while (animating)
        {
            yield return null;
        }
        if (Regex.IsMatch(command, @"^\s*open\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (open)
            {
                yield return "sendtochaterror The death note is already open!";
                yield break;
            }
            buttons[0].OnInteract();
            yield break;
        }
        if (Regex.IsMatch(command, @"^\s*close\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (!open)
            {
                yield return "sendtochaterror The death note is already closed!";
                yield break;
            }
            buttons[1].OnInteract();
            yield break;
        }
        if (Regex.IsMatch(command, @"^\s*enter\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(command, @"^\s*newline\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (!open)
            {
                yield return "sendtochaterror The death note is closed and therefore nothing can be written in the death note!";
                yield break;
            }
            letsandsymbs[39].OnInteract();
            yield break;
        }
        if (Regex.IsMatch(command, @"^\s*delete\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (!open)
            {
                yield return "sendtochaterror The death note is closed and therefore nothing can be written in the death note!";
                yield break;
            }
            if (pageeighttexts[pageeightindex].text.Length == 0)
            {
                yield break;
            }
            letsandsymbs[38].OnInteract();
            yield break;
        }
        if (Regex.IsMatch(command, @"^\s*goon\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (open)
            {
                yield return "sendtochaterror The death note is open and therefore this button cannot be pressed!";
                yield break;
            }
            buttons[4].OnInteract();
            yield break;
        }
        if (Regex.IsMatch(command, @"^\s*return\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (open)
            {
                yield return "sendtochaterror The death note is open and therefore this button cannot be pressed!";
                yield break;
            }
            buttons[5].OnInteract();
            yield break;
        }
        if (Regex.IsMatch(command, @"^\s*passon\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (open)
            {
                yield return "sendtochaterror The death note is open and therefore this button cannot be pressed!";
                yield break;
            }
            buttons[6].OnInteract();
            yield break;
        }
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*press\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (parameters.Length > 2)
            {
                yield return "sendtochaterror Too many parameters!";
            }
            else if (parameters.Length == 2)
            {
                if (parameters[1].EqualsIgnoreCase("goon"))
                {
                    if (open)
                    {
                        yield return "sendtochaterror The death note is open and therefore this button cannot be pressed!";
                        yield break;
                    }
                    buttons[4].OnInteract();
                }
                else if (parameters[1].EqualsIgnoreCase("return"))
                {
                    if (open)
                    {
                        yield return "sendtochaterror The death note is open and therefore this button cannot be pressed!";
                        yield break;
                    }
                    buttons[5].OnInteract();
                }
                else if (parameters[1].EqualsIgnoreCase("passon"))
                {
                    if (open)
                    {
                        yield return "sendtochaterror The death note is open and therefore this button cannot be pressed!";
                        yield break;
                    }
                    buttons[6].OnInteract();
                }
                else
                {
                    yield return "sendtochaterror The specified button to press '" + parameters[1] + "' is invalid!";
                }
            }
            else if (parameters.Length == 1)
            {
                yield return "sendtochaterror Please specify the button to press!";
            }
            yield break;
        }
        if (Regex.IsMatch(parameters[0], @"^\s*delete\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (parameters.Length > 2)
            {
                yield return "sendtochaterror Too many parameters!";
            }
            else if (parameters.Length == 2)
            {
                if (!open)
                {
                    yield return "sendtochaterror The death note is closed and therefore nothing can be written in the death note!";
                    yield break;
                }
                int temp = 0;
                if (!int.TryParse(parameters[1], out temp))
                {
                    yield return "sendtochaterror The specified number of times to delete a character '" + parameters[1] + "' is invalid!";
                    yield break;
                }
                if (temp < 1)
                {
                    yield return "sendtochaterror The specified number of times to delete a character '" + parameters[1] + "' is out of range (< 1)!";
                    yield break;
                }
                for (int j = 0; j < temp; j++)
                {
                    if (pageeighttexts[pageeightindex].text.Length == 0)
                    {
                        yield break;
                    }
                    letsandsymbs[38].OnInteract();
                    yield return new WaitForSeconds(0.1f);
                }
            }
            yield break;
        }
        if (Regex.IsMatch(parameters[0], @"^\s*write\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (parameters.Length == 1)
            {
                yield return "sendtochaterror Please the text that should be written!";
            }
            if (!open)
            {
                yield return "sendtochaterror The death note is closed and therefore nothing can be written in the death note!";
                yield break;
            }
            string[] validStrs = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", " ", ":" };
            string arg = "";
            for (int i = 1; i < parameters.Length; i++)
            {
                for (int j = 0; j < parameters[i].Length; j++)
                {
                    if (!validStrs.Contains((parameters[i][j] + "").ToUpper()))
                    {
                        yield return "sendtochaterror The specified character to write in the death note '"+parameters[i][j]+"' is invalid!";
                        yield break;
                    }
                }
                arg += parameters[i].ToUpper() + " ";
            }
            arg = arg.Trim();
            for (int j = 0; j < arg.Length; j++)
            {
                if (pageeighttexts[pageeightindex].text.Length >= 49)
                {
                    yield break;
                }
                letsandsymbs[Array.IndexOf(validStrs, arg[j] + "")].OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
            yield break;
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        while (cooldown)
        {
            yield return true;
        }
        while (!moduleSolved)
        {
            while (animating)
            {
                yield return true;
            }
            if (die.Count(f => f == true) == 0)
            {
                if (open)
                {
                    buttons[1].OnInteract();
                    while (animating)
                    {
                        yield return true;
                    }
                }
                if (buttonToPress == 0 || buttonToPress == 3)
                {
                    buttons[5].OnInteract();
                }
                else if (buttonToPress == 1 || buttonToPress == 4)
                {
                    buttons[6].OnInteract();
                }
                else
                {
                    buttons[4].OnInteract();
                }
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                if (!open)
                {
                    buttons[0].OnInteract();
                    while (animating)
                    {
                        yield return true;
                    }
                }
                string[] strs = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", " ", ":" };
                bool first = true;
                bool skip = false;
                for (int i = 0; i < die.Count; i++)
                {
                    skip = false;
                    if (die[i])
                    {
                        string create = "";
                        if (timedie[i] != 0)
                        {
                            if (timedie[i] == 1 && (hrs > 12))
                            {
                                skip = true;
                            }
                            else if (timedie[i] == 2 && (hrs > 20))
                            {
                                skip = true;
                            }
                            else if (timedie[i] == 3 && (hrs == 23 && mins > 58))
                            {
                                skip = true;
                            }
                            else if (timedie[i] == 4 && (hrs == 23 && mins > 45))
                            {
                                skip = true;
                            }
                            else
                            {
                                create = getTime(timedie[i]);
                            }
                        }
                        else
                        {
                            create = getTime(0);
                        }
                        if (!skip)
                        {
                            if (!first)
                            {
                                letsandsymbs[39].OnInteract();
                                yield return new WaitForSeconds(0.1f);
                            }
                            else
                            {
                                first = false;
                            }
                            for (int j = 0; j < criminals[i].Length; j++)
                            {
                                letsandsymbs[Array.IndexOf(strs, (criminals[i][j] + "").ToUpper())].OnInteract();
                                yield return new WaitForSeconds(0.1f);
                            }
                            letsandsymbs[36].OnInteract();
                            yield return new WaitForSeconds(0.1f);
                            for (int j = 0; j < create.Length; j++)
                            {
                                letsandsymbs[Array.IndexOf(strs, (create[j] + ""))].OnInteract();
                                yield return new WaitForSeconds(0.1f);
                            }
                            letsandsymbs[36].OnInteract();
                            yield return new WaitForSeconds(0.1f);
                            string tempcause = "";
                            if (correctCause[i] != -1)
                            {
                                if (correctCause[i] == 0)
                                {
                                    switch (UnityEngine.Random.Range(0, 2))
                                    {
                                        case 0: tempcause = "Crushed by Boulder"; break;
                                        case 1: tempcause = "Hit by Car"; break;
                                    }
                                }
                                else if (correctCause[i] == 1)
                                {
                                    switch (UnityEngine.Random.Range(0, 3))
                                    {
                                        case 0: tempcause = "Drowned in Acid"; break;
                                        case 1: tempcause = "Trip on Banana Peel"; break;
                                        case 2: tempcause = "Heart Attack"; break;
                                    }
                                }
                                else if (correctCause[i] == 2)
                                {
                                    switch (UnityEngine.Random.Range(0, 3))
                                    {
                                        case 0: tempcause = "Electric Chair"; break;
                                        case 1: tempcause = "Deadly Disease"; break;
                                        case 2: tempcause = "Bomb Explosion"; break;
                                    }
                                }
                                else if (correctCause[i] == 3)
                                {
                                    switch (UnityEngine.Random.Range(0, 2))
                                    {
                                        case 0: tempcause = "Choked by Old Man"; break;
                                        case 1: tempcause = "Shot Through the Heart"; break;
                                    }
                                }
                            }
                            else
                            {
                                tempcause = allCauses[UnityEngine.Random.Range(0, allCauses.Length)];
                            }
                            for (int j = 0; j < tempcause.Length; j++)
                            {
                                letsandsymbs[Array.IndexOf(strs, (tempcause[j] + "").ToUpper())].OnInteract();
                                yield return new WaitForSeconds(0.1f);
                            }
                        }
                    }
                }
                buttons[1].OnInteract();
                while (animating)
                {
                    yield return true;
                }
                if (buttonToPress == 3)
                {
                    buttons[5].OnInteract();
                }
                else if (buttonToPress == 4)
                {
                    buttons[6].OnInteract();
                }
                else
                {
                    buttons[4].OnInteract();
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
