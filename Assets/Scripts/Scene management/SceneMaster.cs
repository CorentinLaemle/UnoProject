using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneMaster : MonoBehaviour
{
    [SerializeField] private string _webGLOpenPageOnQuit;
    [SerializeField] private Image _loadImage;
    [SerializeField] private float _waitTimeBeforeLoading;
    [SerializeField] private Button _button;

    private static SceneMaster _instance;
    public static SceneMaster GetInstance()
    {
        return _instance;
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
        }
        _instance = this;
    }

    void Start()
    {
#if UNITY_WEBGL
        if (_webGLOpenPageOnQuit == null)
        {
            _webGLOpenPageOnQuit = "about:blank";
        }
#endif
    }

    public void LoadScene(string sceneName)
    {
        if(SceneManager.GetSceneByName(sceneName) != null)
        {
            _button.interactable = false;
            StartCoroutine(LoadAsyncSceneByName(sceneName));
        }
    }

    private IEnumerator LoadAsyncSceneByName(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            _loadImage.fillAmount = asyncLoad.progress;

            if(asyncLoad.progress >= 0.89f)
            {
                yield return new WaitForSeconds(_waitTimeBeforeLoading);
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    public virtual void QuitGame()
    {
#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
        Debug.Log("Quit game");
#endif
#if (UNITY_EDITOR)
        UnityEditor.EditorApplication.isPlaying = false;
#elif (UNITY_STANDALONE) 
    Application.Quit();
#elif (UNITY_WEBGL)
    Application.OpenURL(_webGLOpenPageOnQuit);
#endif
    }
}
