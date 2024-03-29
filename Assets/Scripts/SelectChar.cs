using UnityEngine;

/**
 * Class to do a character selection on Unity3D
 * 
 * @author Jefferson Henrique
 * */
public class SelectChar : MonoBehaviour
{

    // The left marker out of visible scence
    public Transform markerLeft2;
    // The left marker of visible scence
    public Transform markerLeft;
    // The middle marker of visible scence
    public Transform markerMiddle;
    // The right marker of visible scence
    public Transform markerRight;
    // The right marker out of visible scence
    public Transform markerRight2;

    // The characters prefabs to pick
    public Transform[] charsPrefabs;

    // The game objects created to be showed on screen
    private GameObject[] chars;

    // The index of the current character
    private int currentChar = 0;

    void Start()
    {
        // We initialize the chars array
        chars = new GameObject[charsPrefabs.Length];

        // We create game objects based on characters prefabs
        int index = 0;
        foreach (Transform t in charsPrefabs)
        {
            chars[index++] = GameObject.Instantiate(t.gameObject, markerRight2.position, Quaternion.identity) as GameObject;
        }
    }

    public void Previous()
    {
        currentChar--;

        if (currentChar < 0)
        {
            currentChar = 0;
        }
    }

    public void Next()
    {
        currentChar++;

        if (currentChar >= chars.Length)
        {
            currentChar = chars.Length - 1;
        }
    }

    public void DisplayLabelText()
    {
        GameObject selectedChar = chars[currentChar];
        string labelChar = selectedChar.name;
        //GUI.Label(new Rect((Screen.width - 100) / 2, 20, 100, 50), labelChar);
    }

    private void Update()
    {
        // The index of the middle character
        int middleIndex = currentChar;
        // The index of the left character
        int leftIndex = currentChar - 1;
        // The index of the right character
        int rightIndex = currentChar + 1;

        // For each character we set the position based on the current index
        for (int index = 0; index < chars.Length; index++)
        {
            Transform transf = chars[index].transform;

            // If the index is less than left index, the character will dissapear in the left side
            if (index < leftIndex)
            {
                transf.position = Vector3.Lerp(transf.position, markerLeft2.position, Time.deltaTime);

                // If the index is less than right index, the character will dissapear in the right side
            }
            else if (index > rightIndex)
            {
                transf.position = Vector3.Lerp(transf.position, markerRight2.position, Time.deltaTime);

                // If the index is equals to left index, the character will move to the left visible marker
            }
            else if (index == leftIndex)
            {
                transf.position = Vector3.Lerp(transf.position, markerLeft.position, Time.deltaTime);

                // If the index is equals to middle index, the character will move to the middle visible marker
            }
            else if (index == middleIndex)
            {
                transf.position = Vector3.Lerp(transf.position, markerMiddle.position, Time.deltaTime);

                // If the index is equals to right index, the character will move to the right visible marker
            }
            else if (index == rightIndex)
            {
                transf.position = Vector3.Lerp(transf.position, markerRight.position, Time.deltaTime);
            }
        }
    }

    /*void OnGUI()
    {
        // Here we create a button to choose a next char
        if (GUI.Button(new Rect(10, (Screen.height - 50) / 2, 100, 50), "Previous"))
        {
            currentChar--;

            if (currentChar < 0)
            {
                currentChar = 0;
            }
        }

        // Now we create a button to choose a previous char
        if (GUI.Button(new Rect(Screen.width - 100 - 10, (Screen.height - 50) / 2, 100, 50), "Next"))
        {
            currentChar++;

            if (currentChar >= chars.Length)
            {
                currentChar = chars.Length - 1;
            }
        }

        // Shows a label with the name of the selected character
        GameObject selectedChar = chars[currentChar];
        string labelChar = selectedChar.name;
        GUI.Label(new Rect((Screen.width - 100) / 2, 20, 100, 50), labelChar);

        // The index of the middle character
        int middleIndex = currentChar;
        // The index of the left character
        int leftIndex = currentChar - 1;
        // The index of the right character
        int rightIndex = currentChar + 1;

        // For each character we set the position based on the current index
        for (int index = 0; index < chars.Length; index++)
        {
            Transform transf = chars[index].transform;

            // If the index is less than left index, the character will dissapear in the left side
            if (index < leftIndex)
            {
                transf.position = Vector3.Lerp(transf.position, markerLeft2.position, Time.deltaTime);

                // If the index is less than right index, the character will dissapear in the right side
            }
            else if (index > rightIndex)
            {
                transf.position = Vector3.Lerp(transf.position, markerRight2.position, Time.deltaTime);

                // If the index is equals to left index, the character will move to the left visible marker
            }
            else if (index == leftIndex)
            {
                transf.position = Vector3.Lerp(transf.position, markerLeft.position, Time.deltaTime);

                // If the index is equals to middle index, the character will move to the middle visible marker
            }
            else if (index == middleIndex)
            {
                transf.position = Vector3.Lerp(transf.position, markerMiddle.position, Time.deltaTime);

                // If the index is equals to right index, the character will move to the right visible marker
            }
            else if (index == rightIndex)
            {
                transf.position = Vector3.Lerp(transf.position, markerRight.position, Time.deltaTime);
            }
        }
    }*/

}
