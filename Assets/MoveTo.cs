using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using HoloToolkit.Unity;



public class MoveTo : MonoBehaviour
{

    //public Transform goal;
    public Transform goal;
    UnityEngine.AI.NavMeshAgent agent;
    public string stringToEdit;
    GameObject[] abc;
    GameObject[] gos;
    bool isDoingCov = false;
    Vector3 cur_position;
    Vector3 last_position;
    List<GameObject> candidate;
    List<string> turn = new List<string>();
    List<Vector3> location = new List<Vector3>();
    bool keepGoing = false;
    bool isFollowing = true;
    bool isFirst = true;
    DictationRecognizer dictation;
    private NavMeshPath navMeshPath;
    private TextToSpeech textToSpeech;
    bool distracted = false;
    //public GameObject VPACAM;
    bool arrived = false;
    int z = 0;
    string cur_destination;

    void OnGUI()
    {
        // Make a text field that modifies stringToEdit.
        // stringToEdit = GUI.TextField(new Rect(300, 50, 200, 20), stringToEdit, 40);


    }

    void Start()
    {
        candidate = new List<GameObject>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        gos = (GameObject[])FindObjectsOfType(typeof(GameObject));
        textToSpeech = GetComponent<TextToSpeech>();
        //gos = GameObject.FindGameObjectsWithTag("objects");
        navMeshPath = new NavMeshPath();
        cur_position = Camera.main.transform.position;
        last_position = Camera.main.transform.position;
        //VPACAM = GameObject.Find("VPACAM");


        //for (int i = 0; i < gos.Length; i++)
        //{
        //    Debug.Log(gos[i].name);
        //}
        /*
      dictation.DictationResult += (text, medium) =>
      {
          isDoingCov = true;
          myListner(myInput);
      };

       myInput.onEndEdit.AddListener(delegate {
          isDoingCov = true;
          myListner(myInput);
      });*/
    }
    void myListner(string myIn)
    {
        Debug.Log("destination: " + myIn);
        //textToSpeech.StartSpeaking("good");
        if (myIn.ToLower().Equals("yes"))
        {
            Debug.Log("continue to");
            distracted = false;
            diallog(cur_destination);
        }
        else if (myIn.ToLower().Equals("no"))
        {
            textToSpeech.StartSpeaking("Where would you like to go next?");
            Debug.Log("next position");
            arrived = false;
            distracted = false;
        }
        if (keepGoing)
        {
            selection(myIn);
            myIn = "";

        }
        else
        {
            diallog(myIn);
        }

    }
    // Update is called once per frame
    void Update()
    {
        //if (Vector3.Distance(agent.destination, agent.transform.position) < 2.1)
        //{
        //    stringToEdit = "Where do you want to go ?";
        //}

        /*
        if (isFirst) {
            Camera.main.transform.position = new Vector3(VPACAM.transform.position.x, VPACAM.transform.position.y, VPACAM.transform.position.z);
            Camera.main.transform.rotation = new Quaternion(VPACAM.transform.rotation.x, VPACAM.transform.rotation.y, VPACAM.transform.rotation.z, VPACAM.transform.rotation.w);
            isFirst = false;
        }


        if (isFollowing)
        {
            Camera.main.transform.position = new Vector3(VPACAM.transform.position.x, VPACAM.transform.position.y, VPACAM.transform.position.z);
            Camera.main.transform.rotation = new Quaternion(VPACAM.transform.rotation.x, VPACAM.transform.rotation.y, VPACAM.transform.rotation.z, VPACAM.transform.rotation.w);
        }else {

            Camera.main.transform.LookAt(agent.transform);
            if (Vector3.Distance(Camera.main.transform.position, agent.transform.position) > 10.0)
            {
                stringToEdit = "Sir, could you please follow ?";
                agent.destination = Camera.main.transform.position;
            }
        }
        */

        if (isDoingCov == false && agent.velocity.x.Equals(0f) && agent.velocity.z.Equals(0f))
        {
            if (Vector3.Distance(agent.destination, agent.transform.position) < 20.0)
            {
                //stringToEdit = "Where do you want to go ?";
            }
        }
        //if (stringToEdit == "TV"){
        //    agent.destination = goal.position;
        //    stringToEdit = "which tv?";
        //}
        cur_position = Camera.main.transform.position;


        if (arrived && !distracted)
        {
            if (Vector3.Distance(agent.transform.position, agent.destination) < 2)
            {
                textToSpeech.StartSpeaking("We have arrived. Would you like to go somewhere else?");
                arrived = false;
                Debug.Log("Arrived!");
            }

        }
        if (z < location.Count && Vector3.Distance(agent.transform.position, location[z]) < 3)
        {
            textToSpeech.StartSpeaking("Turn " + turn[z] + " here");
            z++;
        }
        if (distracted && Vector3.Distance(agent.transform.position, Camera.main.transform.position) < 3)
        {
            agent.destination = agent.transform.position;
            Debug.Log("too close");
        }
        if (Vector3.Distance(agent.transform.position, Camera.main.transform.position)> 8)
        {
            agent.destination = Camera.main.transform.position;

            distracted = true;
            textToSpeech.StartSpeaking("Would you like to continue to " + cur_destination);

        }
        last_position = cur_position;

    }

    void diallog(string myMsg)
    {
        //Debug.Log("dialog");
        candidate.Clear();
        distracted = false;

        for (int i = 0; i < gos.Length; i++)
        {

            if (myMsg.ToLower().Contains(gos[i].name.ToLower()))
            {
                //agent.destination = gos[i].transform.position;
                candidate.Add(gos[i]);
                Debug.Log("possible candidate name: " + gos[i].name);
            }


        }
        Debug.Log("Number of candidates: " + candidate.Count);
        if (candidate.Count == 0)
        {
            Debug.Log("There is no " + myMsg + ".");
            return;

        }

        else if (candidate.Count == 1)
        {
            //stringToEdit = "Follow me!";
            keepGoing = false;
            cur_destination = myMsg;
            agent.destination = candidate[0].transform.position;

            NavMesh.CalculatePath(agent.transform.position, agent.destination, NavMesh.AllAreas, navMeshPath);
            if (navMeshPath.corners.Length == 2)
            {
                textToSpeech.StartSpeaking("We will go straight to " + candidate[0].name.ToLower());
            }
            else
            {
                directionCompose(navMeshPath);
                Debug.Log("Direction: " + stringToEdit);
                textToSpeech.StartSpeaking(stringToEdit);


            }




            //textToSpeech.StartSpeaking("Would you like to go somewhere else?");

        }

        else if (candidate.Count > 1)
        {
            Debug.Log(candidate.Count);
            //stringToEdit = "Which " + myMsg + " ?";
            keepGoing = true;

        }
    }

    void directionCompose(NavMeshPath _navMeshPath)
    {

        stringToEdit = "We will ";
        Debug.Log("NavMeshPath status: " + _navMeshPath.status);
        Debug.Log("Number of corners: " + _navMeshPath.corners.Length);
        bool first_turn = true;
        int num_turn = _navMeshPath.corners.Length - 2 - 1;
        turn.Clear();
        location.Clear();
        z = 0;
        for (int p = 1; p < _navMeshPath.corners.Length - 1; p++)
        {
            //Debug.Log("P: " + p);
            float dist = Vector3.Distance(_navMeshPath.corners[p + 1], _navMeshPath.corners[p]);
            if (dist < 3)
            {
                num_turn--;
                continue;

            }

            if (p == 1)
            {
                Debug.Log("angle:dds ");
                Debug.Log(Camera.main.transform.position);
                Vector3 targetDir = _navMeshPath.corners[p] - Camera.main.transform.position;
                float angle = Vector3.SignedAngle(targetDir, Camera.main.transform.forward, Vector3.up);
                Debug.Log("angle: " + angle);
                if (angle > 0.0)
                {
                    stringToEdit += "first turn left ";

                }
                else if (angle == 0.0)
                {
                    stringToEdit += "go straight. ";
                }
                else
                {
                    stringToEdit += "first turn right ";

                }

            }

            bool isFirstObj = true;

            for (int l = 0; l < gos.Length; l++)
            {
                //Debug.Log("gos[l].name out " + gos[l].name);

                float temp = Vector3.Distance(_navMeshPath.corners[p], gos[l].transform.position);


                if (temp < 3.0)
                {
                    //Debug.Log("gos[l].name in " + gos[l].name);
                    if (isFirstObj)
                    {
                        Vector3 direction = _navMeshPath.corners[p + 1] - _navMeshPath.corners[p];
                        Debug.Log("direction: " + direction);
                        Vector3 forward = _navMeshPath.corners[p] - _navMeshPath.corners[p - 1];
                        float angle = Vector3.SignedAngle(direction, forward, Vector3.up);
                        Debug.Log("AAngle: " + angle);
                        if (angle > 0.0)
                        {
                            if (first_turn)
                            {
                                stringToEdit += "first turn left at ";
                                first_turn = false;
                            }
                            else
                            {
                                stringToEdit += " and then turn left at ";
                            }
                            turn.Add("left");
                        }
                        else
                        {
                            if (first_turn)
                            {
                                stringToEdit += "first turn right at ";
                                first_turn = false;
                            }
                            else
                            {
                                stringToEdit += " and then turn right at ";
                            }
                            turn.Add("right");
                        }
                        stringToEdit += gos[l].name;
                        isFirstObj = false;
                        location.Add(gos[l].transform.position);
                    }
                }
            }
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = _navMeshPath.corners[p];
            Debug.Log(_navMeshPath.corners[p]);
            //Debug.DrawLine(_navMeshPath.corners[p], _navMeshPath.corners[p + 1], Color.red, 2.5f);
        }
        Debug.Log(num_turn);
        if (num_turn <= 1)
        {
            stringToEdit = stringToEdit.Replace("first", "");
        }
        stringToEdit += " Follow me!";
        arrived = true;
    }

    void selection(string myMsg)
    {
        if (myMsg.Contains("nearest"))
        {

            float nearest = 1000f;
            int nearind = 0;
            for (int i = 0; i < candidate.Count; i++)
            {
                float temp = Vector3.Distance(candidate[i].transform.position, agent.transform.position);
                if (temp < nearest)
                {
                    nearest = temp;
                    nearind = i;
                }
            }

            stringToEdit = "Follow me!";
            keepGoing = false;
            agent.destination = candidate[nearind].transform.position;
            isDoingCov = false;
        }
        else if (myMsg.Contains("near") || myMsg.Contains("next to"))
        {

            for (int i = 0; i < gos.Length; i++)
            {

                if (myMsg.Contains(gos[i].name.ToLower()))
                {
                    //agent.destination = gos[i].transform.position;
                    float nearest = 1000f;
                    int nearind = 0;
                    for (int j = 0; j < candidate.Count; j++)
                    {
                        float temp = Vector3.Distance(candidate[j].transform.position, gos[i].transform.position);
                        if (temp < nearest)
                        {
                            nearest = temp;
                            nearind = j;
                        }
                    }

                    stringToEdit = "Follow me!";
                    keepGoing = false;
                    agent.destination = candidate[nearind].transform.position;
                    isDoingCov = false;
                    break;
                }


            }

        }

        else if (myMsg.Contains("farthermost"))
        {

            float farest = 1000f;
            int farind = 0;
            for (int i = 0; i < candidate.Count; i++)
            {
                float temp = Vector3.Distance(candidate[i].transform.position, agent.transform.position);
                if (temp > farest)
                {
                    farest = temp;
                    farind = i;
                }
            }

            stringToEdit = "Follow me!";
            keepGoing = false;
            agent.destination = candidate[farind].transform.position;
            isDoingCov = false;
        }


    }

}
