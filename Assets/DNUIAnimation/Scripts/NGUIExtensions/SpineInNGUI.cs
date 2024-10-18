// using System.Collections.Generic;
// using UnityEngine;
// using Spine.Unity;
// using System.Linq;

// [ExecuteInEditMode]
// [RequireComponent(typeof(SkeletonAnimation))]
// public class SpineInNGUI : UIWidget
// {
// 	// todo: 如果要更换动画的话，因为 MeshRenderer.enable == false; 可能会有问题。到时候看
 	/*
		这个脚本不好直接用，没有解决问题，其中一个原因是，UIDrawCall 只能接收4倍数量的顶点。如果要改，可能需要这样：
			1. 修改 UIDrawCall, 给它一个标志，表明是渲染传统的UI，还是其他的物体
			2. 如果是其他物体，那么 面片的顶点数量不能自己计算，而是有物体自己传入进去
			3. 自定义的材质这一块的话，可以用 UIpaenl.onCreateMaterial 来指明
		另外，因为 clip 的原因，还需要为这些物体的shader，增加相关的shader, 工作量还是很大的
 	*/
// 	private Mesh _mesh;
// 	private Mesh mesh
// 	{
// 		get
// 		{
// 			if (_mesh != null) return _mesh;

// 			MeshFilter mf = GetComponent<MeshFilter>();
// 			MeshRenderer mr = GetComponent<MeshRenderer>();

// 			_mesh = Application.isPlaying ? mf.mesh : mf.sharedMesh;
// 			if (mr != null) mr.enabled = false;

// 			if (_mesh == null)
// 			{
// 				Debug.LogError($"[error][SpineInNGUI]. _mesh == null");
// 			}

// 			return _mesh;
// 		}
// 	}

// 	protected override void Awake ()
// 	{
// 		base.Awake();

// 		MeshRenderer mr = GetComponent<MeshRenderer>();
// 		if (mr != null)
// 		{
// 			mr.enabled = true;
// 			mMat = Application.isPlaying ? mr.material : mr.sharedMaterial;
// 		}
// 		else Debug.LogError($"[error][SpineInNGUI]. no MeshRenderer");
// 	}

// 	protected override void OnStart ()
// 	{
// 		base.OnStart();
// 		if (panel == null) return;

// 		if (!panel.onCreateDrawCall?.GetInvocationList()?.Contains((UIDrawCall.OnCreateDrawCall)OnCreateDrawCall) ?? true)
// 		{
// 			panel.onCreateDrawCall += OnCreateDrawCall;
// 		}
// 	}

// 	public override void RemoveFromPanel ()
// 	{
// 		if(panel != null) panel.onCreateDrawCall -= OnCreateDrawCall;
// 		base.RemoveFromPanel();
// 	}

// 	public override void OnFill (List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
// 	{
// 		if (mesh == null) return;

// 		List<Vector3> meshV = new List<Vector3>(mesh.vertices);
// 		List<Vector2> meshU = new List<Vector2>(mesh.uv);
// 		List<Color> meshC = new List<Color>(mesh.colors);

// 		if (meshV.Count != meshU.Count || meshV.Count != meshC.Count)
// 		{
// 			Debug.LogError("[error][SpineInNGUI]. mesh vertice count is unmatch");
// 			return;
// 		}

// 		verts.AddRange(meshV);
// 		uvs.AddRange(meshU);
// 		cols.AddRange(meshC);
		
// 		onPostFill?.Invoke(this, verts.Count, verts, uvs, cols);
// 	}

// 	private void OnCreateDrawCall(UIDrawCall dc, MeshFilter filter, MeshRenderer ren)
// 	{
// 		if (mesh == null) return;

// 		Mesh drawCallMesh = Application.isPlaying ? filter.mesh : filter.sharedMesh;
// 		bool isSameMat = drawCallMesh.name.Contains(mMat.name);
// 		if (!isSameMat) return;

// 		int[] drawCallTriangles = drawCallMesh.triangles;
// 		int[] myTriangles = mesh.triangles;
// 		int singleVerCount = mesh.vertices.Length;

// 		// Debug.LogError($"[test][OnCreateDrawCall]. drawCallTriangles: {drawCallTriangles.Length}; myTriangles: {myTriangles.Length}");

// 		if (myTriangles.Length == drawCallTriangles.Length) drawCallMesh.triangles = myTriangles;
// 		else if (drawCallTriangles.Length > myTriangles.Length)
// 		{	
// 			for (int i = 0; i < drawCallTriangles.Length; ++i)
// 			{
// 				int sIdx = i % myTriangles.Length;
// 				int r = i / myTriangles.Length;
// 				drawCallTriangles[i] = myTriangles[sIdx] + r * singleVerCount;
// 			}

// 			drawCallMesh.triangles = drawCallTriangles;
// 		}
// 		else
// 		{
// 			Debug.LogError("[error][OnCreateDrawCall]. from different mesh");
// 		}
// 	}
// }
