using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Logger : MonoBehaviour {

    private static Text log;
    private static Dictionary<string, string> values;

	// Use this for initialization
	void Start () {
        if (!log)
        {
            values = new Dictionary<string, string>();
            Logger.log = GetComponent<Text>();
        }
	}

    void Update()
    {
        string logText = "";
        foreach (string key in values.Keys)
        {
            string value = values[key];
            logText += key + " = " + value + "\n";
        }

        Logger.log.text = logText;
    }

    public static void SetValue(string key, string value)
    {
        values[key] = value;
    }

}
