using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Boggie : MonoBehaviour
{
	[Header("BOARD")]
	public GameObject boardParent;
    List<InputField> alphabetInputFields = new List<InputField>();

    [Header("DICTIONARY")]
    public InputField dictionaryInputField;
	List<string> dictionary = new List<string>();

	[Header("STATS")]
	public Text noOfRepeatedWordsBox;
	int repeatedWords;
	public Text noOfFoundWordsBox;
	int foundWords;

	[Header("ERROR HEADER")]
    public GameObject errorHeader;

	[Header("CONTENT")]
	public Transform Content;

	[Header("WORD PREFAB")]
	public Text word;

	// Let the given dictionary be following
	static readonly int graphIndex = 3;
	char[,] boggle = new char[graphIndex, graphIndex];
	
	void Start()
	{
		errorHeader.SetActive(false);
		noOfRepeatedWordsBox.gameObject.SetActive(false);
		noOfFoundWordsBox.gameObject.SetActive(false);

		foreach(var alphabet in boardParent.GetComponentsInChildren<InputField>())
			alphabetInputFields.Add(alphabet);
	}

    public void OnClickStartButton()
    {
		// CLEAR DATA...
		ClearPreviousData();

		//VERIFY DATA...
		Validation();

		//PROCESS...
		FindWords(boggle);
	}

	//Clear previous found word results
	void ClearPreviousData()
	{
		foreach (var word in GameObject.FindGameObjectsWithTag("BoggleWord"))
			Destroy(word);

		dictionary.Clear();

		repeatedWords = 0;
		noOfRepeatedWordsBox.gameObject.SetActive(false);

		foundWords = 0;
		noOfFoundWordsBox.gameObject.SetActive(false);

		print("Cleared");
	}

	// Validating user data
	void Validation()
	{
		for (int i = 0, k = 0; i < graphIndex; i++)
			for (int j = 0; j < graphIndex; j++)
			{
				if (!string.IsNullOrEmpty(alphabetInputFields[k].text))
				{
					boggle[i, j] = char.Parse(alphabetInputFields[k].text);
					print(boggle[i, j]);
				}
				else
				{
					errorHeader.SetActive(true);
					return;
				}
				k++;
			}

		if (string.IsNullOrEmpty(dictionaryInputField.text))
        {
            errorHeader.SetActive(true);
            return;
        }
        else
            CreateDictionary();

		// Verified
         errorHeader.SetActive(false);
	}

	// Creating dictionary with Input string
    void CreateDictionary()
    {
		foreach (string word in Regex.Split(dictionaryInputField.text, @"\s+"))
		{
			string temp = word.ToUpper();
			if (!dictionary.Contains(temp))
			{
				if (temp.Length >= 4)
					dictionary.Add(temp);
				print("dictionary::" + temp);
			}
			else
			{
				noOfRepeatedWordsBox.gameObject.SetActive(true);
				noOfRepeatedWordsBox.text = (++repeatedWords).ToString();
			}
		}
    }

	// Prints all words present in dictionary. 
	void FindWords(char[,] boggle)
	{
		print("Finding Words");

		// Mark all characters as not visited
		bool[,] visited = new bool[graphIndex, graphIndex];

		// Initialize current string 
		string str = "";

		// Consider every character and look for all words 
		// starting with this character 
		for (int i = 0; i < graphIndex; i++)
			for (int j = 0; j < graphIndex; j++)
				findWordsUtil(boggle, visited, i, j, str);
	}

	// A recursive function to print all words present on boggle 
	void findWordsUtil(char[,] boggle, bool[,] visited, int i, int j, string str)
	{
		// Mark current cell as visited and 
		// append current character to str 
		visited[i, j] = true;
		str = str + boggle[i, j];

		// If str is present in dictionary, 
		// then print it 
		if (dictionary.Contains(str))
		{
			print("Words" + str);
			word.text = str;
			Instantiate(word.gameObject, Content);
			noOfFoundWordsBox.gameObject.SetActive(true);
			noOfFoundWordsBox.text = (++foundWords).ToString();
		}

		// Traverse 8 adjacent cells of boggle[i,j] 
		for (int row = i - 1; row <= i + 1 && row < graphIndex; row++)
			for (int col = j - 1; col <= j + 1 && col < graphIndex; col++)
				if (row >= 0 && col >= 0 && !visited[row, col])
					findWordsUtil(boggle, visited, row, col, str);

		// Erase current character from string and 
		// mark visited of current cell as false 
		str = "" + str[str.Length - 1];
		visited[i, j] = false;
	}
}