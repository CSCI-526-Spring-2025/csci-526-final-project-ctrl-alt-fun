
【CrateMaterial 材质使用说明 - 在 Unity 中操作】

1. 在 Unity 中右键 → Create > Material，命名为 CrateMaterial。
2. 设置贴图：
   - Albedo → T_CrateBigClean_A.png
   - Normal Map → T_CrateBigClean_NM.png（点击 Fix Now）
   - Occlusion → T_CrateBigClean_AO.png
   - Metallic/Gloss → T_CrateBigClean_Rough.png 可用于控制光滑度（调节 Smoothness）
3. 拖动材质至 SM_Crate_Big.fbx 模型上。

也可以使用 URP HDRP Shader 中对应的 PBR 输入贴图。
