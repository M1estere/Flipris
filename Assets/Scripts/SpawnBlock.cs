using UnityEngine;

public class SpawnBlock : MonoBehaviour
{
    [SerializeField] private GameObject[] _blocks;
    [Space(5)]
    
    [SerializeField] private GameManager _gameManager;
    
    private void Start()
    {
        SpawnNewBlock();
    }

    public void SpawnNewBlock()
    {
        var temp = Instantiate(_blocks[Random.Range(0, _blocks.Length)], transform.position, Quaternion.identity);
        
        _gameManager.CurrentBlock = temp.GetComponent<TetrisBlock>();
    }
    
    public void EndGame()
    {
        _gameManager.GameOver();
    }
}
