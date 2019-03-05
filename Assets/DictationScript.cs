
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;

public class DictationScript : MonoBehaviour
{
    [SerializeField]
    private string m_Hypotheses;

    [SerializeField]
    private string m_Recognitions;
    GameObject[] all_objects;

    private TextToSpeech textToSpeech;

    private DictationRecognizer m_DictationRecognizer;

    void Start()
    {
        m_DictationRecognizer = new DictationRecognizer();
        all_objects = (GameObject[])FindObjectsOfType(typeof(GameObject));
        textToSpeech = GetComponent<TextToSpeech>();

        m_DictationRecognizer.DictationResult += (text, confidence) =>
        {
            if (text.ToLower().Equals("yes") || text.ToLower().Equals("no"))
            {
                this.BroadcastMessage("myListner", text.ToLower());
            }
            foreach (GameObject e in all_objects)
            {
                if (text.ToLower().Contains(e.name.ToLower()))
                {
                    this.BroadcastMessage("myListner", e.name.ToLower());
                    Debug.LogFormat("found game object ", text);
                    //textToSpeech.StartSpeaking("Follow me");
                    //textToSpeech.StartSpeaking(e.name);
                }
                else
                {


                    //var msg = string.Format(speakText, textToSpeech.Voice.ToString());
                    //textToSpeech.StartSpeaking(msg);
                    //Debug.LogFormat("game object does not exist ", text);
                }
            }
            m_Recognitions = text + "\n";
        };

        m_DictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;

        m_DictationRecognizer.DictationComplete += (completionCause) =>
        {
            if (completionCause != DictationCompletionCause.Complete)
                Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", completionCause);
        };

        m_DictationRecognizer.DictationError += (error, hresult) =>
        {
            Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
        };

        m_DictationRecognizer.Start();
    }
    void DictationRecognizer_DictationHypothesis(string text)
    {
        //Debug.LogFormat("Dictation hypothesis: ", text);
        m_Hypotheses = text;
    }

}