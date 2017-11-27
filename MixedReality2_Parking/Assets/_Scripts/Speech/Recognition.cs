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
            playerController.GoForward();
        });
        keywords.Add("Turn straight", () =>
        {
            playerController.TurnForward();
        });

        keywords.Add("Go left", () =>
        {
            playerController.GoLeft();
        });
        keywords.Add("Turn left", () =>
        {
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

        keywords.Add("Search", () =>
        {
            playerController.SearchParkingSpaces();
        });

        keywords.Add("Hide", () =>
        {
            playerController.HideParkingSpaceDisplay();
        });

        keywords.Add("1", () =>
        {
            playerController.Select(1);
        });
        keywords.Add("2", () =>
        {
            playerController.Select(2);
        });
        keywords.Add("3", () =>
        {
            playerController.Select(3);
        });
        keywords.Add("4", () =>
        {
            playerController.Select(4);
        });
        keywords.Add("5", () =>
        {
            playerController.Select(5);
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
}
