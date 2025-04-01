using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;

[System.Serializable]
public class AnalyticsEvent {
    public string sessionId;
    public string playerId;
    public string eventType;
    public long timestamp;
    public string levelId;
    public int eventSequence;
    public string viewBeforeEvent;
}

[System.Serializable]
public class AnalyticsEventPayload {
    public List<AnalyticsEvent> events;
}

public class AnalyticsManager : MonoBehaviour {

    public static AnalyticsManager instance { get; private set; }
    private List<AnalyticsEvent> eventsCache = new List<AnalyticsEvent>();
    private string playerId;

    public bool isEnabled = false;

    // Sending interval in seconds
    public float sendInterval = 5f;
    
    // Server API address
    public bool isProduction = false;
    private string apiUrl = "https://side-n-top-default-rtdb.firebaseio.com/test/.json";
    private string apiUrlProduction = "https://side-n-top-default-rtdb.firebaseio.com/events/.json";

    void Awake() {
        // Keep AnalyticsManager object
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        playerId = Guid.NewGuid().ToString();
        if (isEnabled)
        {
            string currentMode = "Test Mode: ";
            if (isProduction) {
                apiUrl = apiUrlProduction;
                currentMode = "Production Mode: ";
            }
            Debug.Log(currentMode + "Analytics Manager starts sending data from player " + playerId);
            StartCoroutine(SendEventsRoutine());
        }
        else
        {
            Debug.Log("Analytics Manager is not enabled for player: " + playerId);
        }

    }

    // For external: add an event into cache
    public void AddAnalyticsEvent(string sessionId, string eventType, string levelId, long timestamp, int eventSequence, string viewBeforeEvent) {
        AnalyticsEvent newEvent = new AnalyticsEvent() {
            sessionId = sessionId,
            playerId = playerId,
            eventType = eventType,
            levelId = levelId,
            timestamp = timestamp,
            eventSequence = eventSequence,
            viewBeforeEvent = viewBeforeEvent
        };

        // Debug.Log("New event:" + newEvent);
        eventsCache.Add(newEvent);
    }

    // Check cache and send data after sendInterval seconds
    IEnumerator SendEventsRoutine() {
        while (true) {
            yield return new WaitForSeconds(sendInterval);

            // Check if the cache is empty
            if (eventsCache.Count > 0) {
                yield return StartCoroutine(SendEvents());
            }
            // AddAnalyticsEvent("sessionId", "playerId", "eventType", "levelId", 11111, 11, "viewBeforeEvent", "result");
            // yield return StartCoroutine(SendEvents());
        }
    }

    // Send data in cache to server
    IEnumerator SendEvents() {
        AnalyticsEventPayload payload = new AnalyticsEventPayload();
        payload.events = new List<AnalyticsEvent>(eventsCache);

        string json = JsonUtility.ToJson(payload);
        byte[] jsonToSend = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success) {
            Debug.Log("Failure sending analytics data: " + request.error);
            // Not clear cache after failure sending
        } else {
            Debug.Log("Success sending analytics data!");
            // Clear cache after success sending
            eventsCache.Clear();
        }
    }
}
