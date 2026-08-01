using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapLegendUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private MapControllerV2 mapController;
    [SerializeField] private GameObject legendRowPrefab;
    [SerializeField] private Transform colorsContainer;
    [SerializeField] private Transform iconsContainer;

    [Header("Sprite base para colores planos")]
    [SerializeField] private Sprite plainSquareSprite; // un sprite blanco cuadrado, para tintar con color

    private struct LegendEntry
    {
        public string label;
        public Color color;
        public Sprite icon;
        public bool isColor; // true = pintar por color, false = usar icono tal cual
    }

    private void OnEnable()
    {
        BuildLegend();
    }

    [ContextMenu("Build Legend")]
    public void BuildLegend()
    {
        ClearContainer(colorsContainer);
        ClearContainer(iconsContainer);

        List<LegendEntry> colorEntries = new List<LegendEntry>
        {
            new LegendEntry { label = "Suelo",   color = mapController.FloorColor,  isColor = true },
            new LegendEntry { label = "Muro",    color = mapController.WallColor,   isColor = true },
            new LegendEntry { label = "Oxígeno", color = mapController.OxygenColor, isColor = true },
        };

        List<LegendEntry> iconEntries = new List<LegendEntry>
        {
            new LegendEntry { label = "NPC",       icon = mapController.NpcSprite,      isColor = false },
            new LegendEntry { label = "Flor",      icon = mapController.FlowerSprite,   isColor = false },
            new LegendEntry { label = "Hoguera",   icon = mapController.CampfireSprite, isColor = false },
            new LegendEntry { label = "Puerta",    icon = mapController.GateSprite,     isColor = false },
        };

        foreach (LegendEntry entry in colorEntries)
            CreateRow(entry, colorsContainer);

        foreach (LegendEntry entry in iconEntries)
        {
            if (entry.icon == null) continue; // evita filas vacías si algún sprite no está asignado
            CreateRow(entry, iconsContainer);
        }
    }

    private void CreateRow(LegendEntry entry, Transform container)
    {
        GameObject row = Instantiate(legendRowPrefab, container);

        Image image = row.GetComponentInChildren<Image>();
        TextMeshProUGUI label = row.GetComponentInChildren<TextMeshProUGUI>();

        if (entry.isColor)
        {
            image.sprite = plainSquareSprite;
            image.color = entry.color;
        }
        else
        {
            image.sprite = entry.icon;
            image.color = Color.white;
        }

        label.text = entry.label;
    }

    private void ClearContainer(Transform container)
    {
        for (int i = container.childCount - 1; i >= 0; i--)
            Destroy(container.GetChild(i).gameObject);
    }
}