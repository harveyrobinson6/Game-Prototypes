using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using N_Grid;

public class UIManager : MonoBehaviour
{
    [SerializeField] BattleSceneManager BSM;
    [SerializeField] TextMeshProUGUI TerrainName;
    [SerializeField] TextMeshProUGUI TerrainDesc;
    [SerializeField] TextMeshProUGUI MovementModifier;
    [SerializeField] TextMeshProUGUI EvasionModifier;
    [SerializeField] Transform MovementBox;
    [SerializeField] Transform EvasionBox;

    private void Start()
    {
        MovementBox.gameObject.SetActive(false);
        EvasionBox.gameObject.SetActive(false);
    }

    public void CursorFeedback(G_Tile tile)
    {
        MovementBox.gameObject.SetActive(false);
        EvasionBox.gameObject.SetActive(false);

        foreach (var item in BSM.TerrainTypes)
        {
            if (item.TerrainName == tile.TileType.ToString())
            {
                TerrainType terrainType = item;
                TerrainName.text = terrainType.TerrainName;
                TerrainDesc.text = terrainType.TerrainDesc;

                if (terrainType.TerrainHinderance != 0 || terrainType.TerrainEvasion != 0)
                {
                    MovementBox.gameObject.SetActive(true);
                    EvasionBox.gameObject.SetActive(true);

                    MovementModifier.text = terrainType.TerrainHinderance.ToString();
                    EvasionModifier.text = terrainType.TerrainEvasion.ToString() + "%";
                }

                break;
            }
        }
    }
}
