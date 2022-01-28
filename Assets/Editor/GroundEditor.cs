using UnityEditor;

[CustomEditor(typeof(GroundGenerator))]
public class GroundEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GroundGenerator groundGenerator = target as GroundGenerator;
        groundGenerator.GenerateInitialGround();
    }

}
