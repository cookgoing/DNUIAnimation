using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class TestMaterial : MonoBehaviour
{
	public Renderer objectRenderer;
	public MaterialPropertyBlock materialBlock;

	void Start()
	{
		materialBlock = new MaterialPropertyBlock();
		materialBlock.SetColor("_Color", Color.red);

		print($"activeColorSpace: {QualitySettings.activeColorSpace}");
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			TestMaterialCreate();
		}

		if (Input.GetKeyDown(KeyCode.B))
		{
			TestMaterialBlock(false);
		}

		if (Input.GetKeyDown(KeyCode.C))
		{
			TestMaterialBlock(true);
		}
		
	}

	void TestMaterialCreate()
	{
		 // 获取原始材质
			Material originalMaterial = objectRenderer.sharedMaterial;

			// 打印初始材质
			Debug.Log("Original shared Material: " + originalMaterial.GetInstanceID());
			print("Original Material: " + objectRenderer.material.GetInstanceID());

			// 修改材质的颜色
			objectRenderer.material.color = Color.red;

			// 获取修改后的材质
			Material newMaterial = objectRenderer.material;

			// 打印修改后的材质ID
			Debug.Log("New Material: " + newMaterial.GetInstanceID());

			// 再次修改材质
			objectRenderer.material.color = Color.green;

			// 获取第二次修改后的材质
			Material secondNewMaterial = objectRenderer.material;

			// 打印第二次修改后的材质ID
			Debug.Log("Second New Material: " + secondNewMaterial.GetInstanceID());
	}

	void TestMaterialBlock(bool clear)
	{
		if (clear) objectRenderer.SetPropertyBlock(null);
		else objectRenderer.SetPropertyBlock(materialBlock);
	}
}