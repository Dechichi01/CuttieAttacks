using UnityEngine;
using System.Collections;

[System.Serializable]
public class SpawnRect {

    public Rect rect;
    [SerializeField]
    private Transform owner;
    public void SetOwner(Transform transform) { owner = transform; }

    public SpawnRect(float x, float y, float width, float height, Transform _owner)
    {
        rect.x = x;
        rect.y = y;
        rect.width = width;
        rect.height = height;
        SetOwner(_owner);
    }

    public Vector3 bottomLeft { get { return new Vector3(owner.position.x + rect.x - rect.width / 2, owner.position.y, owner.position.z + rect.y - rect.height / 2); } }
    public Vector3 topLeft { get { return new Vector3(owner.position.x + rect.x - rect.width / 2, owner.position.y, owner.position.z + rect.y + rect.height / 2); } }
    public Vector3 bottomRight { get { return new Vector3(owner.position.x + rect.x + rect.width / 2, owner.position.y, owner.position.z + rect.y - rect.height / 2); } }
    public Vector3 topRight { get { return new Vector3(owner.position.x + rect.x + rect.width / 2, owner.position.y, owner.position.z + rect.y + rect.height / 2); } }

    public void DrawRectangle()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(topRight, topLeft);
    }
}
