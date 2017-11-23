using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class Recognition : MonoBehaviour {
    [SerializeField]
    private PlayerController playerController;

   private KeywordRecognizer keywordRecognizer;
    [SerializeField]
    private Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();
	// Use this for initialization
	void Start () {
        keywords.Add("Go straight", () =>
        {
            Debug.Log("Go straight");
            playerController.GoForward();
        });
        keywords.Add("Turn straight", () =>
        {
            Debug.Log("Turn straight");
            playerController.TurnForward();
        });

        string goLeft = "Go left";
        keywords.Add(goLeft, () =>
        {
            Debug.Log(goLeft);
            playerController.GoLeft();
        });
        keywords.Add("Turn left", () =>
        {
            Debug.Log("Turn forward");
            playerController.TurnLeft();
        });
        keywords.Add("Go right", () =>
        {
            playerController.GoRight();
        });
        keywords.Add("Turn right", () =>
        {
            playerController.TurnRight();
        });
        keywords.Add("Go back", () =>
        {
            playerController.GoBackward();
        });
        keywords.Add("Turn back", () =>
        {
            playerController.TurnBackward();
        });
        keywords.Add("Continue", () =>
        {
            playerController.Continue();
        });
        


        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizerOnPhraseRecognized;
        keywordRecognizer.Start();
        
		
	}

   public void KeywordRecognizerOnPhraseRecognized(PhraseRecognizedEventArgs args) {
        System.Action keywordAction;

        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
            Debug.Log(args.text);
        }

    }

   public void CalledAkash() {
        print("You just said Akash!");
    }

    public void CalledRoshan()
    {
        print("You just said Roshan!");
    }


}
