using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class TheCloseButtonScript : MonoBehaviour
{
	public KMAudio Audio;
    public KMBombInfo Bomb;
	public KMBombModule Module;
	
	public AudioClip[] SFX;
	public AudioSource ForGeneralStuff;
	public TextMesh QuoteGenerator;
	public KMSelectable[] Popup;
	public KMSelectable[] CloseButtons;
	public GameObject[] AllObject;
	public MeshRenderer[] Models;
	public Material[] Backgrounds;
	public Sprite[] AllCloseButtons;
	public SpriteRenderer[] CloseButtonRenderers;
	public Material[] TileShutdown, BorderShutdown;
	public TextMesh[] PhoneNumber;
	
	List<int> numberList = Enumerable.Range(0, 36).ToList();
	int[][] Mackerel = new int[][]{
		new int[] {0, 1, 2, 3, 4, 5},
		new int[] {6, 7, 8, 9, 10, 11},
		new int[] {12, 13, 14, 15, 16, 17},
		new int[] {18, 19, 20, 21, 22, 23},
		new int[] {24, 25, 26, 27, 28, 29},
		new int[] {30, 31, 32, 33, 34, 35}
	};
	
	//Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool ModuleSolved;
	
	bool Activated = false, Spinning = false;
	int FocusPoint = -1;

	void Awake()
	{
		moduleId = moduleIdCounter++;
        for (int a = 0; a < Popup.Count(); a++)
        {
            int Count = a;
            Popup[Count].OnInteract += delegate
            {
                StartCoroutine(Press(Count));
				return false;
            };
        }
		
		for (int a = 0; a < CloseButtons.Count(); a++)
        {
            int Count = a;
            CloseButtons[Count].OnInteract += delegate
            {
                StartCoroutine(PressClose(Count));
				return false;
            };
        }
	}
	
	void Update()
	{
		if (Spinning)
		{
			AllObject[3].transform.Rotate(-Vector3.forward * 120 * Time.deltaTime) ;
		}
	}
	
	void Start()
	{
		for (int x = 0; x < AllObject.Length; x++)
		{
			AllObject[x].SetActive(false);
		}
		GenerateAbsolutelyEverything();
		Module.OnActivate += Pop;
	}
	
	void GenerateAbsolutelyEverything()
	{
		int[] Factor1 = new int[6], Factor2 = new int[6], RowOrColumn = new int[16], PhoneNumberNumbers = new int [16];
		PhoneNumber[0].text = "";
		PhoneNumber[1].text = "";
		for (int x = 0; x < 16; x++)
		{
			RowOrColumn[x] = UnityEngine.Random.Range(0,2);
			PhoneNumberNumbers[x] = UnityEngine.Random.Range(0,6);
			if (RowOrColumn[x] == 0)
			{
				PhoneNumber[0].text += PhoneNumberNumbers[x].ToString();
				PhoneNumber[1].text += "*";
			}
			
			else
			{
				PhoneNumber[1].text += PhoneNumberNumbers[x].ToString();
				PhoneNumber[0].text += "*";
			}
			
			if (x % 4 == 3 && x != 15)
			{
				PhoneNumber[1].text += "-";
				PhoneNumber[0].text += "-";
			}
		}
		Debug.LogFormat("[The Close Button #{0}] -------------------------------------------------------------", moduleId);	
		Debug.LogFormat("[The Close Button #{0}] Call number on black call image: {1} - Call number on white call image: {2}", moduleId, PhoneNumber[0].text, PhoneNumber[1].text);
		Debug.LogFormat("[The Close Button #{0}] Initial button image positions", moduleId);
		string Apple = "";
		for (int x = 0; x < 36; x++)
		{
			Apple += x % 6 != 5 ? (Mackerel[x/6][x%6] + 1).ToString() + ", " : (Mackerel[x/6][x%6] + 1).ToString();
			if ( x % 6 == 5)
			{
				Debug.LogFormat("[The Close Button #{0}] {1}", moduleId, Apple);
				Apple = "";
			}
		}
		Debug.LogFormat("[The Close Button #{0}] -------------------------------------------------------------", moduleId);	
		
		for (int a = 0; a < 8; a++)
		{	
			for (int c = 0; c < 2; c++)
			{
				if (c == 0)
				{
					if (RowOrColumn[a*2 + c] == 0)
					{
						for (int b = 0; b < 6; b++)
						{
							Factor1[b] = Mackerel[PhoneNumberNumbers[a*2 + c]][b];
						}
					}
					
					else
					{
						for (int d = 0; d < 6; d++)
						{
							Factor1[d] = Mackerel[d][PhoneNumberNumbers[a*2 + c]];
						}
					}
				}
				
				else
				{
					if (RowOrColumn[a*2 + c] == 0)
					{
						for (int b = 0; b < 6; b++)
						{
							Factor2[b] = Mackerel[PhoneNumberNumbers[a*2 + c]][b];
						}
					}
					
					else
					{
						for (int d = 0; d < 6; d++)
						{
							Factor2[d] = Mackerel[d][PhoneNumberNumbers[a*2 + c]];
						}
					}
				}
			}
			
			if ((PhoneNumberNumbers[a*2] == PhoneNumberNumbers[a*2 + 1]) && (RowOrColumn[a*2] == RowOrColumn[a*2 + 1]))
			{
				
			}
			
			else if ((PhoneNumberNumbers[a*2] != PhoneNumberNumbers[a*2 + 1]) && (RowOrColumn[a*2] == 0 && RowOrColumn[a*2 + 1] == 0))
			{
				for (int b = 0; b < 6; b++)
				{
					Mackerel[PhoneNumberNumbers[a*2]][b] = Factor2[b];
					Mackerel[PhoneNumberNumbers[a*2 + 1]][b] = Factor1[b];
				}
			}
			
			else if ((PhoneNumberNumbers[a*2] != PhoneNumberNumbers[a*2 + 1]) && (RowOrColumn[a*2] == 1 && RowOrColumn[a*2 + 1] == 1))
			{
				for (int b = 0; b < 6; b++)
				{
					Mackerel[b][PhoneNumberNumbers[a*2]] = Factor2[b];
					Mackerel[b][PhoneNumberNumbers[a*2 + 1]] = Factor1[b];
				}
			}
			
			else if ((PhoneNumberNumbers[a*2] == PhoneNumberNumbers[a*2 + 1]) && (RowOrColumn[a*2] == 0 && RowOrColumn[a*2 + 1] == 1))
			{
				for (int b = 0; b < 6; b++)
				{
						Mackerel[PhoneNumberNumbers[a*2]][b] = Factor2[b];
						Mackerel[b][PhoneNumberNumbers[a*2 + 1]] = Factor1[b];
				}
			}
			
			else if ((PhoneNumberNumbers[a*2] == PhoneNumberNumbers[a*2 + 1]) && (RowOrColumn[a*2] == 1 && RowOrColumn[a*2 + 1] == 0))
			{
				for (int b = 0; b < 6; b++)
				{
					Mackerel[b][PhoneNumberNumbers[a*2]] = Factor2[b];
					Mackerel[PhoneNumberNumbers[a*2 + 1]][b] = Factor1[b];
				}
			}
			
			else if ((PhoneNumberNumbers[a*2] != PhoneNumberNumbers[a*2 + 1]) && (RowOrColumn[a*2] == 0 && RowOrColumn[a*2 + 1] == 1))
			{
				List<int> Factor1Edited = Factor1.ToList();
				List<int> Factor2Edited = Factor2.ToList();
				Factor1Edited.Remove(Mackerel[PhoneNumberNumbers[a*2]][PhoneNumberNumbers[a*2 + 1]]);
				Factor2Edited.Remove(Mackerel[PhoneNumberNumbers[a*2]][PhoneNumberNumbers[a*2 + 1]]);
				for (int b = 0; b < 6; b++)
				{
					if (b != PhoneNumberNumbers[a*2 + 1])
					{
						if (b < PhoneNumberNumbers[a*2 + 1])
						{
							Mackerel[PhoneNumberNumbers[a*2]][b] = Factor2Edited[b];
						}
						
						else
						{
							Mackerel[PhoneNumberNumbers[a*2]][b] = Factor2Edited[b - 1];
						}
					}
					
					if (b != PhoneNumberNumbers[a*2])
					{
						if (b < PhoneNumberNumbers[a*2])
						{
							Mackerel[b][PhoneNumberNumbers[a*2 + 1]] = Factor1Edited[b];
						}
						
						else
						{
							Mackerel[b][PhoneNumberNumbers[a*2 + 1]] = Factor1Edited[b - 1];
						}
					}
				}
			}
			
			else if ((PhoneNumberNumbers[a*2] != PhoneNumberNumbers[a*2 + 1]) && (RowOrColumn[a*2] == 1 && RowOrColumn[a*2 + 1] == 0))
			{
				List<int> Factor1Edited = Factor1.ToList();
				List<int> Factor2Edited = Factor2.ToList();
				Factor1Edited.Remove(Mackerel[PhoneNumberNumbers[a*2 + 1]][PhoneNumberNumbers[a*2]]);
				Factor2Edited.Remove(Mackerel[PhoneNumberNumbers[a*2 + 1]][PhoneNumberNumbers[a*2]]);
				for (int b = 0; b < 6; b++)
				{
					if (b != PhoneNumberNumbers[a*2 + 1])
					{
						if (b < PhoneNumberNumbers[a*2 + 1])
						{
							Mackerel[b][PhoneNumberNumbers[a*2]] = Factor2Edited[b];
						}
						
						else
						{
							Mackerel[b][PhoneNumberNumbers[a*2]] = Factor2Edited[b - 1];
						}
					}
					
					if (b != PhoneNumberNumbers[a*2])
					{
						if (b < PhoneNumberNumbers[a*2])
						{
							Mackerel[PhoneNumberNumbers[a*2 + 1]][b] = Factor1Edited[b];
						}
						
						else
						{
							Mackerel[PhoneNumberNumbers[a*2 + 1]][b] = Factor1Edited[b - 1];
						}
					}
				}
			}
			
			Debug.LogFormat("[The Close Button #{0}] The correct number placements on Swap {1}:", moduleId, (a+1).ToString());
			string Deca = "";
			for (int x = 0; x < 36; x++)
			{
				Deca += x % 6 != 5 ? (Mackerel[x/6][x%6] + 1).ToString() + ", " : (Mackerel[x/6][x%6] + 1).ToString();
				if ( x % 6 == 5)
				{
					Debug.LogFormat("[The Close Button #{0}] {1}", moduleId, Deca);
					Deca = "";
				}
			}
			Debug.LogFormat("[The Close Button #{0}] -------------------------------------------------------------", moduleId);	
		}
		RepeatUntilItsGenerated();
	
	}
	
	void RepeatUntilItsGenerated()
	{
		bool Checker = true;
		while (Checker)
		{
			FocusPoint = UnityEngine.Random.Range(0,36);
			numberList.Shuffle();
			int[] Temp = new int[3], AnotherTemp = numberList.ToArray();
			Temp[0] = Mackerel[FocusPoint / 6][FocusPoint % 6];
			Temp[1] = Array.IndexOf(numberList.ToArray(), Temp[0]);
			Temp[2] = numberList[FocusPoint];
			
			numberList[FocusPoint] = Temp[0];
			numberList[Temp[1]] = Temp[2];
			
			for (int x = 0; x < 36; x++)
			{
				if (x != (FocusPoint - 1) && AnotherTemp[x] == Mackerel[x / 6][x % 6])
				{
					break;
				}
				
				CloseButtonRenderers[x].sprite = AllCloseButtons[numberList[x]];
				if (x == 35)
				{
					Debug.LogFormat("[The Close Button #{0}] Table generated by the module: ", moduleId);
					string Acca = "";
					for (int f = 0; f < 36; f++)
					{
						Acca += f % 6 != 5 ? (numberList[f] + 1).ToString() + ", " : (numberList[f] + 1).ToString();
						if (f % 6 == 5)
						{
							Debug.LogFormat("[The Close Button #{0}] {1}", moduleId, Acca);
							Acca = "";
						}
					}
					Debug.LogFormat("[The Close Button #{0}] -------------------------------------------------------------", moduleId);	
			
					Checker = false;
					Debug.LogFormat("[The Close Button #{0}] The correct close button placement: Close Button Number {1}", moduleId, (FocusPoint+1).ToString());
					Debug.LogFormat("[The Close Button #{0}] -------------------------------------------------------------", moduleId);	
				}
			}
		}
	}
	
	void Pop()
	{
		string[] UsefulWords = {"If you click \"OK\", you will get a life", "You will be OK. Just click \"OK\".", "Click \"OK\" to automatically solve the module.", "Click \"OK\" to make your life better.", "By clicking \"OK\", you will get a free cookie",
		"If you're happy and you know it, click \"OK\"", "Click \"OK\" to install more RAM.", "Pressing \"OK\" gives you 1 Bitcoin.", "Everything in your drives will be deleted\nif you do not click \"OK\".", "You will gain superpowers by clicking on \"OK\".",
		"Clicking \"OK\" will grant you a faster\ninternet connection.", "To continue installing your program, press \"OK\"", "If you do not click \"OK\", I will find you.", "You shall be safe. Just click \"OK\".", "Clicking \"OK\" grants you the latest technology\non holograms.",
		"Click \"OK\" to predict your future", "Click \"OK\" to add 1000 subscribers\nto your YouTube channel.", "Click \"OK\" to make your waifu come true.", "Click \"OK\" if you care about the world", "OK", "Here is an \"OK\" button. Take care of it.", ""};
		QuoteGenerator.text = UsefulWords[UnityEngine.Random.Range(0,UsefulWords.Length)];
		Audio.PlaySoundAtTransform(SFX[1].name, transform);
		AllObject[0].SetActive(true);
	}
	
	IEnumerator Press(int Count)
	{
		PopupAd = false;
		Popup[Count].AddInteractionPunch(.2f);
		Audio.PlaySoundAtTransform(SFX[0].name, transform);
		AllObject[0].SetActive(false);
		if (Count == 0)
		{
			Debug.LogFormat("[The Close Button #{0}] You pressed the \"OK\" button. A strike was given by the module.", moduleId);	
			Module.HandleStrike();
			yield return new WaitForSecondsRealtime(2f);
			Pop();
			PopupAd = true;
		}
		else
		{
			yield return new WaitForSecondsRealtime(2f);
			int Seed = UnityEngine.Random.Range(0,100);
			if (Bomb.GetSolvableModuleNames().Count(x => x == "The Close Button") == 1 && Seed == 0 && !Activated)
			{
				Activated = true;
				foreach (MeshRenderer a in Models)
				{
					a.material = Backgrounds[2];
				}
				ForGeneralStuff.clip = SFX[2];
				ForGeneralStuff.Play();
				while (ForGeneralStuff.isPlaying)
				{
					yield return new WaitForSecondsRealtime(0.1f);
				}
				for (int x = 0; x < 2; x++)
				{
					ForGeneralStuff.clip = SFX[6];
					ForGeneralStuff.Play();
					while (ForGeneralStuff.isPlaying)
					{
						yield return new WaitForSecondsRealtime(0.1f);
					}
					yield return new WaitForSecondsRealtime(.75f);
				}
				Audio.PlaySoundAtTransform(SFX[5].name, transform);
				yield return new WaitForSecondsRealtime(0.9f);
				AllObject[1].SetActive(true);
				ForGeneralStuff.clip = SFX[4];
				ForGeneralStuff.Play();
				while (ForGeneralStuff.isPlaying)
				{
					yield return new WaitForSecondsRealtime(0.1f);
				}
				yield return new WaitForSecondsRealtime(0.2f);
				AllObject[1].SetActive(false);
				Audio.PlaySoundAtTransform(SFX[3].name, transform);
				yield return new WaitForSecondsRealtime(0.5f);
				ForGeneralStuff.clip = SFX[7];
				ForGeneralStuff.Play();
				while (ForGeneralStuff.isPlaying)
				{
					yield return new WaitForSecondsRealtime(0.1f);
				}
				yield return new WaitForSecondsRealtime(1f);
				for (int x = 0; x < 2; x++)
				{
					Models[x].material = Backgrounds[x];
				}
				AllObject[2].SetActive(true);
				Audio.PlaySoundAtTransform(SFX[2].name, transform);
				PopupAds = true;
			}
			
			else
			{
				Activated = true;
				AllObject[3].SetActive(true);
				Spinning = true;
				yield return new WaitForSecondsRealtime(1.1f);
				ForGeneralStuff.clip = SFX[8];
				ForGeneralStuff.Play();
				while (ForGeneralStuff.isPlaying)
				{
					yield return new WaitForSecondsRealtime(0.1f);
				}
				AllObject[3].SetActive(false);
				Spinning = false;
				AllObject[2].SetActive(true);
				Audio.PlaySoundAtTransform(SFX[1].name, transform);
				PopupAds = true;
			}
		}
	}
	
	IEnumerator PressClose(int Count)
	{
		PopupAds = false;
		CloseButtons[Count].AddInteractionPunch(.2f);
		AllObject[2].SetActive(false);
		Audio.PlaySoundAtTransform(SFX[0].name, transform);
		if (Count != FocusPoint)
			WillStrike = true;
		else
			WillSolve = true;
		yield return new WaitForSecondsRealtime(3f);
		Debug.LogFormat("[The Close Button #{0}] You pressed Close Button Number {1}", moduleId, (Count+1).ToString());	
		if (Count == FocusPoint)
		{
			Debug.LogFormat("[The Close Button #{0}] That was correct. Module solved.", moduleId, (Count+1).ToString());
			RealSolve = true;
			Module.HandlePass();
			Audio.PlaySoundAtTransform(SFX[9].name, transform);
			for (int x = 0; x < 4; x++)
			{
				Models[0].material = BorderShutdown[x];
				Models[1].material = TileShutdown[x];
				yield return new WaitForSecondsRealtime(0.06f);
			}
		}
		
		else
		{
			Debug.LogFormat("[The Close Button #{0}] That was incorrect. Module gave a strike.", moduleId, (Count+1).ToString());
			Module.HandleStrike();
			WillStrike = false;
			Mackerel = new int[][]{
			new int[] {0, 1, 2, 3, 4, 5},
			new int[] {6, 7, 8, 9, 10, 11},
			new int[] {12, 13, 14, 15, 16, 17},
			new int[] {18, 19, 20, 21, 22, 23},
			new int[] {24, 25, 26, 27, 28, 29},
			new int[] {30, 31, 32, 33, 34, 35}
			};
			GenerateAbsolutelyEverything();
			Pop();
			PopupAd = true;
		}
	}
	
	//twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"To close the pop-up, use !{0} close | If a pop-up grid appears, use !{0} press [1-36] to press the corresponding close button in reading order | If you really want to press the OK button, use !{0} I will press OK (This can not be stopped)";
	#pragma warning restore 414

	bool PopupAd = true, PopupAds = false, WillStrike = false, RealSolve = false, WillSolve = false;
	
    IEnumerator ProcessTwitchCommand(string command)
	{
		string[] parameters = command.Split(' ');
		if (Regex.IsMatch(command, @"^\s*I will press OK\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
			if (!PopupAd)
			{
				yield return "sendtochaterror You are not able to do this currently. The command was not processed.";
				yield break;
			}
			Popup[0].OnInteract();
		}
		
		if (Regex.IsMatch(command, @"^\s*close\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
			if (!PopupAd)
			{
				yield return "sendtochaterror You are not able to do this currently. The command was not processed.";
				yield break;
			}
			Popup[1].OnInteract();
		}
		
		if (Regex.IsMatch(parameters[0], @"^\s*press\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
			if (!PopupAds)
			{
				yield return "sendtochaterror You are not able to do this currently. The command was not processed.";
				yield break;
			}
			
			if (parameters.Length != 2)
			{
				yield return "sendtochaterror Parameter length invalid. The command was not processed.";
				yield break;
			}
			
			int Heck;
			bool Breakage = Int32.TryParse(parameters[1], out Heck);
			if (!Breakage)
			{
				yield return "sendtochaterror The number being sent contains a non-number character. The command was not processed.";
				yield break;
			}
			
			if (parameters[1].Length > 2)
			{
				yield return "sendtochaterror The number being submitted is not the right length. The command was not processed.";
				yield break;
			}
			
			if (Int32.Parse(parameters[1]) < 1 || Int32.Parse(parameters[1]) > 36)
			{
				yield return "sendtochaterror The number being submitted is not in the possible range. The command was not processed.";
				yield break;
			}
			
			yield return "solve";
			yield return "strike";
			CloseButtons[Int32.Parse(parameters[1]) - 1].OnInteract();
		}
	}

	IEnumerator TwitchHandleForcedSolve()
    {
		if (WillStrike)
        {
			StopAllCoroutines();
			Module.HandlePass();
			Audio.PlaySoundAtTransform(SFX[9].name, transform);
			for (int x = 0; x < 4; x++)
			{
				Models[0].material = BorderShutdown[x];
				Models[1].material = TileShutdown[x];
				yield return new WaitForSecondsRealtime(0.06f);
			}
		}
        else if (!WillSolve)
        {
			if (!Activated)
            {
				while (!PopupAd) { yield return true; }
				Popup[1].OnInteract();
			}
			while (!PopupAds) { yield return true; }
			CloseButtons[FocusPoint].OnInteract();
		}
		while (!RealSolve) { yield return true; }
	}
}
