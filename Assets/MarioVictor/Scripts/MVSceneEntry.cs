using UnityEngine;

// El componente serializado en cada una de las siete escenas.
// No depende de GameManager, PlayerMovement ni otros scripts de la plantilla docente.
public sealed class MVSceneEntry : MonoBehaviour
{
    [Range(0, 6)] public int stageId;

    private void Awake()
    {
        MVGameController game = gameObject.AddComponent<MVGameController>();
        game.stageId = stageId;
    }
}
