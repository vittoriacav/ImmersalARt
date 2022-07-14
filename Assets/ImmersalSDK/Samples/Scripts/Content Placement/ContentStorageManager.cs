using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;

namespace Immersal.Samples.ContentPlacement
{
    public class ContentStorageManager : MonoBehaviour
    {
        [HideInInspector]
        public List<MovableContent> contentList = new List<MovableContent>();
        [SerializeField]
        private GameObject PrefabCaption = null;
        [SerializeField]
        private GameObject PrefabPaint = null;
        [SerializeField]
        private Immersal.AR.ARSpace m_ARSpace;
        [SerializeField]
        private string m_Filename = "content.json";
        private Savefile m_Savefile;
        public List<ContentToStore> MetadataToStore = new List<ContentToStore>();

        public GameObject InputFieldCaption;
        public GameObject DropdownMenuPaints;

        private Quaternion Rotation4Prefab;
        private Vector3 Position4Prefab;

        public Shader basicShader;

        private GameObject CaptionToEdit;
        private int progressiveID = 1;


        [System.Serializable]
        public struct ContentToStore
        {
            public Vector3 positionCTS;
            public Quaternion orientationCTS;
            public string nameCTS;
            public string tagCTS;
            public string captionCTS;
        }

        [System.Serializable]
        public struct Savefile
        {
            public List<ContentToStore> MetadataToStoreSF;
        }

        public static ContentStorageManager Instance
        {
            get
            {
#if UNITY_EDITOR
                if (instance == null && !Application.isPlaying)
                {
                    instance = UnityEngine.Object.FindObjectOfType<ContentStorageManager>();
                }
#endif
                if (instance == null)
                {
                    Debug.LogError("No ContentStorageManager instance found. Ensure one exists in the scene.");
                }
                return instance;
            }
        }

        private static ContentStorageManager instance = null;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            if (instance != this)
            {
                Debug.LogError("There must be only one ContentStorageManager object in a scene.");
                UnityEngine.Object.DestroyImmediate(this);
                return;

            }

            if (m_ARSpace == null)
            {
                m_ARSpace = GameObject.FindObjectOfType<Immersal.AR.ARSpace>();
            }
        }



        void Start()
        {
            PrefabPaint.GetComponent<Animation>().enabled = false;

            InputFieldCaption.SetActive(false);
            DropdownMenuPaints.SetActive(false);

            contentList.Clear();
            LoadContents();
            
        }

        public void RaycastPaint()
        {

            Transform cameraTransform = Camera.main.transform;
            if (Physics.Raycast(cameraTransform.localPosition, cameraTransform.forward, out var hit))
            {
                
                DropdownMenuPaints.SetActive(true);

                Rotation4Prefab = cameraTransform.rotation;

                Rotation4Prefab.eulerAngles = Rotation4Prefab.eulerAngles + new Vector3(-90.0f, 0.0f, 0.0f);


                Position4Prefab = hit.point;


            }
            else
            {
                DropdownMenuPaints.SetActive(true);

                Rotation4Prefab = cameraTransform.rotation;

                Rotation4Prefab.eulerAngles = Rotation4Prefab.eulerAngles + new Vector3(-90.0f, 0.0f, 0.0f);


                Position4Prefab = cameraTransform.forward;
                Debug.Log("Did not Hit");
            }

        }

        public void SelectDropdownMenuPaints(int val)
        {
            if (val == 1)
            {
                Debug.Log("VG");
                AddPaint("VG");
            }
            if (val == 2)
            {
                Debug.Log("KH");
                AddPaint("KH");
            }
            if (val == 3)
            {
                Debug.Log("CM");
                AddPaint("CM");
            }
            if (val == 4)
            {
                Debug.Log("DA");
                AddPaint("DA");
            }

            DropdownMenuPaints.SetActive(false);

        }

        public void AddPaint(string namePaint)
        {
            GameObject paint = Instantiate(PrefabPaint, Position4Prefab, Rotation4Prefab, m_ARSpace.transform);
            paint.name = namePaint;
            paint.tag = "paint";

            if (namePaint == "DA")
            {
                paint.transform.localScale = new Vector3(0.09f, 1.0f, 0.09f);
            }

            paint.GetComponent<Animation>().enabled = true;
            var loadImg = Resources.Load<Texture2D>("MatPaintings/" + paint.name + "/" + paint.name + "0");

            Material mat = new Material(basicShader);
            mat.mainTexture = loadImg;
            MeshRenderer mr = paint.GetComponent<MeshRenderer>();
            mr.material = mat;

            MovableContent other = (MovableContent)paint.GetComponent(typeof(MovableContent));
            other.StoreContent();
        }
        public void RaycastMainCaption()
        {
            

            Transform cameraTransform = Camera.main.transform;
            if (Physics.Raycast(cameraTransform.localPosition, cameraTransform.forward, out var hit))
            {
                Rotation4Prefab = cameraTransform.rotation;
                Position4Prefab = hit.point;
                AddCaption("MainCaption", null, progressiveID);
                progressiveID += 1;

            }
            else
            {
                Rotation4Prefab = cameraTransform.rotation;
                Position4Prefab = cameraTransform.forward;
                AddCaption("MainCaption", null, progressiveID);
                progressiveID += 1;
                Debug.Log("Did not Hit");
            }

        }

        public void RaycastExtensionCaption()
        {

            Transform cameraTransform = Camera.main.transform;

            int layerMask = LayerMask.GetMask("TransparentFX");
            layerMask = ~layerMask;


            if (Physics.Raycast(cameraTransform.localPosition, cameraTransform.TransformDirection(Vector3.forward), out var hit, Mathf.Infinity, layerMask))
            {
               
                GameObject parent = hit.collider.gameObject;
                if (parent.transform.childCount == 0)
                {
                    Position4Prefab = parent.transform.position + Vector3.up / 3;
                    Rotation4Prefab = parent.transform.rotation;
                    AddCaption("extended", parent);
                }
                else
                {
                    GameObject child = parent.transform.GetChild(0).gameObject;
                    if (child.activeSelf)
                    {
                        child.SetActive(false);
                    }
                    else
                    {
                        child.SetActive(true);
                    }

                }

            }
            else
            {
                Debug.Log("Did not Hit");
            }
        }



        public void AddCaption(string nameCaption, GameObject parent = null, int ID = 0)
        {
            GameObject caption = Instantiate(PrefabCaption, Position4Prefab, Rotation4Prefab, m_ARSpace.transform);

            caption.tag = "caption";

            if (parent!= null)
            {
                caption.transform.parent = parent.transform;
                if(parent.name.Substring(0,11) == "MainCaption")
                {
                    caption.name = nameCaption + parent.name.Substring(11) +":0";
                }
                else
                {
                    int loc = parent.name.IndexOf(":");
                    int updateIDextension = int.Parse(parent.name.Substring(loc + 1))+1;
                    caption.name = nameCaption + parent.name.Substring(8, loc-8) + ":" + updateIDextension.ToString();
                }
            }
            else
            {
                caption.name = nameCaption+ID;
                
            }

            MovableContent other = (MovableContent)caption.GetComponent(typeof(MovableContent));
            other.StoreContent();
            CaptionToEdit = caption;

        }



        public void WriteCaption1()
        {
            Transform cameraTransform = Camera.main.transform;
            int layerMask = LayerMask.GetMask("TransparentFX");
            layerMask = ~layerMask;
            if (Physics.Raycast(cameraTransform.localPosition, cameraTransform.TransformDirection(Vector3.forward), out var hit, Mathf.Infinity, layerMask))
            {
                CaptionToEdit = hit.collider.gameObject;
                InputFieldCaption.SetActive(true);
            }
            else
            {

                Debug.Log("DID NOT HIT");
            }
        }

        public void WriteCaption2(string LoadText = null)
        {
            if (string.IsNullOrEmpty(LoadText))
            {
                GameObject TextChild = InputFieldCaption.transform.Find("Text Area/Text").gameObject;
                LoadText = TextChild.GetComponent<TextMeshProUGUI>().text;
            }
            
            CaptionToEdit.GetComponent<TextMeshPro>().text = LoadText;
            InputFieldCaption.SetActive(false);
            MovableContent other = (MovableContent)CaptionToEdit.GetComponent(typeof(MovableContent));
            other.StoreContent();

        }

        public void DeleteAllContent()
        {
            List<MovableContent> copy = new List<MovableContent>();


            foreach (MovableContent content in contentList)
            {
                copy.Add(content);
            }

            foreach (MovableContent content in copy)
            {
                content.RemoveContent();
            }
        }

        public void SaveContents()
        {
            MetadataToStore.Clear();
            foreach (MovableContent content in contentList)
            {
                var temp = new ContentToStore
                {
                    positionCTS = content.transform.localPosition,
                    orientationCTS = content.transform.localRotation,
                    nameCTS = content.gameObject.name,
                    tagCTS = content.gameObject.tag
                };

                if(content.gameObject.CompareTag("caption"))
                {
                    temp.captionCTS = content.gameObject.GetComponent<TextMeshPro>().text;
                }

                MetadataToStore.Add(temp);
            }
            m_Savefile.MetadataToStoreSF = MetadataToStore;

            string jsonstring = JsonUtility.ToJson(m_Savefile, true);
            string dataPath = Path.Combine(Application.persistentDataPath, m_Filename);
            File.WriteAllText(dataPath, jsonstring);
        }

        public void LoadContents()
        {
            string dataPath = Path.Combine(Application.persistentDataPath, m_Filename);
            Debug.Log(string.Format("Trying to load file: {0}", dataPath));

            try
            {

                Savefile loadFile = JsonUtility.FromJson<Savefile>(File.ReadAllText(dataPath));
                List<ContentToStore> ExtensionsList = new List<ContentToStore>();
                            
                foreach (ContentToStore data in loadFile.MetadataToStoreSF)
                {
                    Position4Prefab = data.positionCTS;
                    Rotation4Prefab = data.orientationCTS;

                    if (data.tagCTS == "paint")
                    {
                        AddPaint(data.nameCTS);
                    }
                    else //tag = "caption"
                    {
                        if(data.nameCTS.Substring(0,11) == "MainCaption")
                        {
                            AddCaption("MainCaption", null, int.Parse(data.nameCTS.Substring(11)));
                            WriteCaption2(data.captionCTS);
                            if (int.Parse(data.nameCTS.Substring(11)) > progressiveID)
                            {
                                progressiveID = int.Parse(data.nameCTS.Substring(11))+1;
                            }
                        }
                        else
                        {
                            ExtensionsList.Add(data);
                        }
                    }
                }


                foreach (ContentToStore ext in ExtensionsList)
                {
                    int loc = ext.nameCTS.IndexOf(":");
                    string IDmain = ext.nameCTS.Substring(8, loc - 8);

                    if (ext.nameCTS.Substring(loc + 1) == "0")
                    {
                        MovableContent parent = contentList.Find(x => x.gameObject.name == "MainCaption" + IDmain);
                        Position4Prefab = parent.transform.position + Vector3.up / 3;
                        Rotation4Prefab = parent.transform.rotation;
                        AddCaption("extended", parent.gameObject);
                        WriteCaption2(ext.captionCTS);
                    }
                    else
                    {
                        int PrevExt = int.Parse(ext.nameCTS.Substring(loc + 1)) - 1;
                        
                        MovableContent parent = contentList.Find(x => x.gameObject.name == "extended" + IDmain + ":" + PrevExt.ToString());
                        Position4Prefab = parent.transform.position + Vector3.up / 3;
                        Rotation4Prefab = parent.transform.rotation;
                        AddCaption("extended", parent.gameObject);
                        WriteCaption2(ext.captionCTS);
                    }

                }

                foreach(MovableContent mc in contentList)
                {
                    if(mc.gameObject.name.Substring(0,2) == "Ma")
                    {
                        if (mc.gameObject.transform.childCount != 0)
                        {
                            GameObject child = mc.gameObject.transform.GetChild(0).gameObject;
                            child.SetActive(false);
                        }
                        
                    }
                }


                Debug.Log("Successfully loaded file!");
            }
            catch (FileNotFoundException e)
            {
                Debug.Log(e.Message + "\n.json file for content storage not found. Created a new file!");
                File.WriteAllText(dataPath, "");
            }  

        }

    }
}
