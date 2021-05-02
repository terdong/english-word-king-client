//#define LOG_TRACE_INFO
//#define LOG_EXTRA_INFO

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TeamGehem.DataModels.Protocols;

//------------------------------------------------------------------------------
// class definition
//------------------------------------------------------------------------------
public class MainController : MonoBehaviour
{
	private static MainController mainController = null;

    public Camera root_camera_;

    private Stack<string> prevSceneName_stack;
	private string currentSceneName_;
	private string nextSceneName_;
	private AsyncOperation resourceUnloadTask;
    private AsyncOperation sceneLoadTask = null;
    public AsyncOperation SceneLoadTask
    {
        private get { return sceneLoadTask; }
        set { sceneLoadTask = value; }
    }
	private enum SceneState { Reset, Preload, Load, Unload, Postload, Ready, Run, Count };
	private SceneState sceneState;
	private delegate void UpdateDelegate();
	private UpdateDelegate[] updateDelegates;
    private CommonUI common_ui_ = null;
    private NetworkManager network_manager_;
    private DataManager data_manager_;
    // Scene 중복 이동 방지를 위한 Lock 변수.[Terdong : 2014-08-04]
    private bool isLockFormMoveToScene;

	//--------------------------------------------------------------------------
	// public static methods
	//--------------------------------------------------------------------------

    public static void SwitchScene( string nextSceneName )
    {
        MainController.SwitchScene( nextSceneName, true );
    }

    public static void SwitchScene(SceneListEnum scene_list)
    {
        MainController.SwitchScene(scene_list.ToString(), true);
    }

	public static void SwitchScene(string nextSceneName, bool isKeepCurrentScene)
	{
		if(mainController != null)
		{
			if( mainController.currentSceneName_ != nextSceneName && mainController.isLockFormMoveToScene == false )
			{
                mainController.isLockFormMoveToScene = true;

                if ( isKeepCurrentScene )
                {
                    mainController.PushNextSceneToPrevSceneStack();
                }
                else
                {
                    Stack<string> prevSceneName_stack = mainController.prevSceneName_stack;
                    prevSceneName_stack.Pop();
                    Debug.Log( "Stack_Scene count = " + prevSceneName_stack.Count );
                }
				mainController.nextSceneName_ = nextSceneName;
			}
		}
	}

    public static void MovePrevScene()
    {
        Stack<string> prevSceneName_stack = mainController.prevSceneName_stack;
        if ( prevSceneName_stack.Count > 0 )
        {
            MainController.SwitchScene( prevSceneName_stack.Peek(), false );
        }
    }

    public static void SetSceneLoadTask( AsyncOperation sceneLoadTask )
    {
        mainController.sceneLoadTask = sceneLoadTask;
    }

    public static CommonUI GetCommonUI()
    {
        return mainController == null ? null : mainController.common_ui_;
    }

	//--------------------------------------------------------------------------
	// protected mono methods
	//--------------------------------------------------------------------------
	protected void Awake()
	{
		//Let's keep this alive between scene changes
		Object.DontDestroyOnLoad(gameObject);

		//Setup the singleton instance
		mainController = this;

		//Setup the array of updateDelegates
		updateDelegates = new UpdateDelegate[(int)SceneState.Count];

		//Set each updateDelegate
		updateDelegates[(int)SceneState.Reset] = UpdateSceneReset;
		updateDelegates[(int)SceneState.Preload] = UpdateScenePreload;
		updateDelegates[(int)SceneState.Load] = UpdateSceneLoad;
		updateDelegates[(int)SceneState.Unload] = UpdateSceneUnload;
		updateDelegates[(int)SceneState.Postload] = UpdateScenePostload;
		updateDelegates[(int)SceneState.Ready] = UpdateSceneReady;
		updateDelegates[(int)SceneState.Run] = UpdateSceneRun;

		nextSceneName_ = "Menu_Scene";
		sceneState = SceneState.Reset;
        //root_camera_.orthographicSize = Screen.width / 2;
        Screen.SetResolution(1080, 1920, true);

        prevSceneName_stack = new Stack<string>();

        common_ui_ = transform.FindChild( "Common_UI" ).GetComponent<CommonUI>();
        network_manager_ = transform.FindChild( "Manager" ).GetComponent<NetworkManager>();
        data_manager_ = transform.FindChild( "Manager" ).GetComponent<DataManager>();
        isLockFormMoveToScene = false;
	}

	protected void OnDestroy()
	{
		//Clean up all the updateDelegates
		if(updateDelegates != null)
		{
			for(int i = 0; i < (int)SceneState.Count; i++)
			{
				updateDelegates[i] = null;
			}
			updateDelegates = null;
		}

		//Clean up the singleton instance
		if(mainController != null)
		{
            Destroy(mainController);
			mainController = null;
		}
	}

    //protected void OnDisable() {}

    //protected void OnEnable() {}

    //protected void Start() {}

	protected void Update()
	{
		if(updateDelegates[(int)sceneState] != null)
		{
			updateDelegates[(int)sceneState]();
		}
        //Debug.Log(Screen.currentResolution.width + ", " + Screen.currentResolution.height);
	}

	//--------------------------------------------------------------------------
	// private methods
	//--------------------------------------------------------------------------
	// attach the new scene controller to start cascade of loading
	private void UpdateSceneReset()
	{
		// run a gc pass
		System.GC.Collect();
		sceneState = SceneState.Preload;
	}

	// handle anything that needs to happen before loading
	private void UpdateScenePreload()
	{
        //sceneLoadTask = Application.LoadLevelAsync( nextSceneName );

        //TODO : 페이드 아웃 시, 이전 Scene을 Hide 처리해야함. [Terdong : 2014-08-04]
        AutoFade.LoadLevel( nextSceneName_, 0.5f, 0.5f, Color.black );
		sceneState = SceneState.Load;
	}

	// show the loading screen until it's loaded
	private void UpdateSceneLoad()
	{
		// done loading?
        if ( sceneLoadTask != null && sceneLoadTask.isDone == true )
		{
			sceneState = SceneState.Unload;
            sceneLoadTask = null;
		}
		else
		{
			// update scene loading progress

		}
	}

	// clean up unused resources by unloading them
	private void UpdateSceneUnload()
	{
		// cleaning up resources yet?
		if(resourceUnloadTask == null)
		{
			resourceUnloadTask = Resources.UnloadUnusedAssets();
		}
		else
		{
			// done cleaning up?
			if(resourceUnloadTask.isDone == true)
			{
				resourceUnloadTask = null;
				sceneState = SceneState.Postload;
			}
		}
	}

	// handle anything that needs to happen immediately after loading
	private void UpdateScenePostload()
	{
		currentSceneName_ = nextSceneName_;
		sceneState = SceneState.Ready;
	}

	// handle anything that needs to happen immediately before running
	private void UpdateSceneReady()
	{
		// run a gc pass
		// if you have assets loaded in the scene that are
		// currently unused currently but may be used later
		// DON'T do this here
		System.GC.Collect();
		sceneState = SceneState.Run;

        // 뒤로가기 버튼 Lock 해제.[Terdong : 2014-08-04]
        StartCoroutine( "UnLockMoveToPrevScene" );
	}

	// wait for scene change
	private void UpdateSceneRun()
	{
		if(currentSceneName_ != nextSceneName_)
		{
			sceneState = SceneState.Reset;
		}
	}

    private void PushNextSceneToPrevSceneStack()
    {
        prevSceneName_stack.Push( nextSceneName_ );
        Debug.Log( "Stack_Scene count = " + prevSceneName_stack.Count );
    }

    IEnumerator UnLockMoveToPrevScene()
    {
        yield return new WaitForSeconds( 0.5f );
        isLockFormMoveToScene = false;
    }
}
