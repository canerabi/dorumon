
#pragma warning disable 649
#pragma warning disable 168
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
public partial class Base2:MonoBehaviour
{    
    static ServersWindow __ServersWindow;
    public static ServersWindow _ServersWindow { get { if (__ServersWindow == null) __ServersWindow = (ServersWindow)MonoBehaviour.FindObjectOfType(typeof(ServersWindow)); return __ServersWindow; } }
}

public class ServersWindow : WindowBase {
		
	public string Ipaddress{ get { return PlayerPrefs.GetString("Ipaddress", @""); } set { PlayerPrefs.SetString("Ipaddress", value); } }
	public int Port{ get { return PlayerPrefs.GetInt("Port", 5300); } set { PlayerPrefs.SetInt("Port", value); } }
	internal bool focus���������_�����������;
	internal bool enabled���������_����������� = true;
	internal bool focusIpaddress;
	internal bool isReadOnlyIpaddress = false;
	internal bool focusPort;
	internal bool isReadOnlyPort = false;
	internal bool focusConnect;
	internal bool Connect=false;
	internal bool focus������_��������;
	internal bool enabled������_�������� = true;
	internal bool focusServersTable;
	internal string[] ServersTable = new string[] {};
	internal int iServersTable = -1;
	internal bool focusRefresh;
	internal bool Refresh=false;
	internal bool focusServersTitle;
	internal bool isReadOnlyServersTitle = true;
	internal string ServersTitle = @"  ��������_�������         �����           ���_����          �������        ����";
	private int wndid1;
	private bool oldMouseOverConnect;
	private Vector2 sServersTable;
	private bool oldMouseOverRefresh;
	
    
    
	void Start () {
		wndid1 = UnityEngine.Random.Range(0, 1000);

	}    
    
    
    bool focusWindow;
    void OnEnable()
    {
        focusWindow = true;
    }
    
    void OnGUI()
    {		
		GUI.skin = (GUISkin)Resources.Load("Skin/Skin");
        
		GUI.Window(wndid1,new Rect(-326.5f + Screen.width/2,-287f + Screen.height/2,618f,492f), Wnd1,"");

    }
	void Wnd1(int id){
		if (focusWindow) {GUI.FocusWindow(id);GUI.BringWindowToFront(id);}
		focusWindow = false;
		bool onMouseOver;
		if(focus���������_�����������) { focus���������_����������� = false; GUI.FocusControl("���������_�����������");}
		GUI.SetNextControlName("���������_�����������");
		if(enabled���������_�����������){
		GUI.BeginGroup(new Rect(8f, 27f, 329f, 81f), "");
		GUI.Box(new Rect(0, 0, 329f, 81f), "");
		GUI.Label(new Rect(21f, 30f, 66.89667f, 21.96f), @"ip �������");
		if(focusIpaddress) { focusIpaddress = false; GUI.FocusControl("Ipaddress");}
		GUI.SetNextControlName("Ipaddress");
		if(isReadOnlyIpaddress){
		GUI.Label(new Rect(103.3f, 30f, 123f, 14f), Ipaddress.ToString());
		} else
		Ipaddress = GUI.TextField(new Rect(103.3f, 30f, 123f, 14f), Ipaddress);
		GUI.Label(new Rect(64.38f, 48f, 32.32333f, 21.96f), @"port");
		if(focusPort) { focusPort = false; GUI.FocusControl("Port");}
		GUI.SetNextControlName("Port");
		if(isReadOnlyPort){
		GUI.Label(new Rect(103.3f, 48f, 123f, 14f), Port.ToString());
		} else
		Port = int.Parse(GUI.TextField(new Rect(103.3f, 48f, 123f, 14f), Port.ToString()));
		if(focusConnect) { focusConnect = false; GUI.FocusControl("Connect");}
		GUI.SetNextControlName("Connect");
		bool oldConnect = Connect;
		Connect = GUI.Button(new Rect(244f, 30f, 75f, 32f), new GUIContent("���������",""));
		if (Connect != oldConnect && Connect ) {Action("onConnect");onButtonClick(); }
		onMouseOver = new Rect(244f, 30f, 75f, 32f).Contains(Event.current.mousePosition);
		if (oldMouseOverConnect != onMouseOver && onMouseOver) onOver();
		oldMouseOverConnect = onMouseOver;
		GUI.EndGroup();
		}
		if(focus������_��������) { focus������_�������� = false; GUI.FocusControl("������_��������");}
		GUI.SetNextControlName("������_��������");
		if(enabled������_��������){
		GUI.BeginGroup(new Rect(8f, 112f, 602f, 355f), "");
		GUI.Box(new Rect(0, 0, 602f, 355f), "");
		if(focusServersTable) { focusServersTable = false; GUI.FocusControl("ServersTable");}
		GUI.SetNextControlName("ServersTable");
		GUI.Box(new Rect(8f, 48f, 586f, 299f), "");
		sServersTable = GUI.BeginScrollView(new Rect(8f, 48f, 586f, 299f), sServersTable, new Rect(0,0, 566f, ServersTable.Length* 15f));
		int oldServersTable = iServersTable;
		iServersTable = GUI.SelectionGrid(new Rect(0,0, 566f, ServersTable.Length* 15f), iServersTable, ServersTable,1,GUI.skin.customStyles[0]);
		if (iServersTable != oldServersTable) Action("onServersTable",ServersTable[iServersTable]);
		GUI.EndScrollView();
		GUI.Label(new Rect(0f, 0f, 114.45f, 14f), @"������ ��������");
		if(focusRefresh) { focusRefresh = false; GUI.FocusControl("Refresh");}
		GUI.SetNextControlName("Refresh");
		bool oldRefresh = Refresh;
		Refresh = GUI.Button(new Rect(512f, 4.04f, 82f, 21.96f), new GUIContent("Refresh",""));
		if (Refresh != oldRefresh && Refresh ) {Action("onRefresh");onButtonClick(); }
		onMouseOver = new Rect(512f, 4.04f, 82f, 21.96f).Contains(Event.current.mousePosition);
		if (oldMouseOverRefresh != onMouseOver && onMouseOver) onOver();
		oldMouseOverRefresh = onMouseOver;
		if(focusServersTitle) { focusServersTitle = false; GUI.FocusControl("ServersTitle");}
		GUI.SetNextControlName("ServersTitle");
		if(isReadOnlyServersTitle){
		GUI.Label(new Rect(8f, 30f, 586f, 14f), ServersTitle.ToString(), GUI.skin.customStyles[2]);
		} else
		ServersTitle = GUI.TextField(new Rect(8f, 30f, 586f, 14f), ServersTitle, GUI.skin.customStyles[2]);
		GUI.Box(new Rect(173f, 30f, 1f, 317f),"",GUI.skin.customStyles[4]);
		GUI.Box(new Rect(282f, 30f, 1f, 318.907f),"",GUI.skin.customStyles[4]);
		GUI.Box(new Rect(404f, 30f, 1f, 318.511f),"",GUI.skin.customStyles[4]);
		GUI.Box(new Rect(523f, 30f, 1f, 318.191f),"",GUI.skin.customStyles[4]);
		GUI.EndGroup();
		}
		GUI.Label(new Rect(8f, 27f, 56.61f, 14f), @"�������");
		if (GUI.Button(new Rect(618f - 25, 5, 20, 15), "X")) { enabled = false;onButtonClick();ActionAll("onClose"); }
	}


	void Update () {
	
	}
}