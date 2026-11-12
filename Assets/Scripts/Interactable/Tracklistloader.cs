using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class TrackListLoader : MonoBehaviour
{
    public string API_URL = "http://localhost:8080/dsaApp/tracks";

    public TextMeshProUGUI container;

    [Header("Panell (Canvas en World Space)")]
    public GameObject panel;
    public float height = 2f;
    public bool faceCamera = true;

    private bool loading = false;

    private void Start()
    {
        if (panel != null) panel.SetActive(false);
    }

    private void LateUpdate()
    {
        if (panel == null || !panel.activeSelf) return;

        panel.transform.position = transform.position + Vector3.up * height;

        if (faceCamera && Camera.main != null)
        {
            Vector3 dir = panel.transform.position - Camera.main.transform.position;
            dir.y = 0; 
            if (dir.sqrMagnitude > 0.001f)
                panel.transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    // Aquesta és la funció que s'ha d'afegir a "On Interact ()" del cub.
    // 1a interacció: mostra el panell i carrega els tracks.
    // 2a interacció: amaga el panell.
    public void FetchTracks()
    {
        if (panel != null && panel.activeSelf)
        {
            panel.SetActive(false);
            return;
        }

        if (panel != null) panel.SetActive(true);
        if (!loading) StartCoroutine(FetchTracksCoroutine());
    }

    private IEnumerator FetchTracksCoroutine()
    {
        loading = true;
        container.text = "<color=#333333>carregant canciones...</color>";

        using (UnityWebRequest response = UnityWebRequest.Get(API_URL))
        {
            yield return response.SendWebRequest();

            if (response.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error al cargar tracks: " + response.error);
                container.text = "<color=red><align=center>No se pudo conectar con la API en "
                                 + API_URL + "</align></color>";
            }
            else
            {

                string json = "{\"tracks\":" + response.downloadHandler.text + "}";
                TrackList data = JsonUtility.FromJson<TrackList>(json);

                // container.innerHTML = "";
                string html = "<size=130%><b><color=#007BFF>Lista de Canciones</color></b></size>\n\n";

                
                foreach (Track track in data.tracks)
                {
                    string title = string.IsNullOrEmpty(track.title) ? "Unknown Track" : track.title;
                    string singer = string.IsNullOrEmpty(track.singer) ? "Unknown Artist" : track.singer;
                    string id = track.id == null ? "" : track.id.ToUpper();

                    html += "<size=120%><b><color=#1A1A1A>" + title + "</color></b></size>\n";   // <h3>
                    html += "<color=#666666>" + singer + "</color>\n";                           // <p>
                    html += "<size=70%><color=#888888><mark=#F0F2F5AA> REF: " + id
                            + " </mark></color></size>\n\n";                                      // <small>
                }

                container.text = html;
            }
        }

        loading = false;
    }
}

[System.Serializable]
public class Track
{
    public string id;
    public string title;
    public string singer;
}

[System.Serializable]
public class TrackList
{
    public Track[] tracks;
}