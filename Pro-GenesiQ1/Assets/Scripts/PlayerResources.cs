using System;
using TMPro;
using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    public static PlayerResources Instance;
    
    [SerializeField] private TextMeshProUGUI textWood;
    [SerializeField] private TextMeshProUGUI textStone;
    [SerializeField] private TextMeshProUGUI textFood;
    
    public int wood;
    public int stone;
    public int food;

    private void Awake()
    {
        Instance = this;
        textWood.text = wood.ToString();
        textStone.text = stone.ToString();
        textFood.text = food.ToString();
    }
    
    public void AddResource(ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.Wood:
                wood += amount;
                if (wood < 0) wood = 0;
                textWood.text = wood.ToString();
                break;
            case ResourceType.Stone:
                stone += amount;
                if (stone < 0) stone = 0;
                textStone.text = stone.ToString();
                break;
            case ResourceType.Food:
                food += amount;
                if (food < 0) food = 0;
                textFood.text = food.ToString();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
    public bool SpendResource(ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.Wood:
                if (wood >= amount)
                {
                    wood -= amount;
                    textWood.text = wood.ToString();
                    return true;
                }
                break;
            case ResourceType.Stone:
                if (stone >= amount)
                {
                    stone -= amount;
                    textStone.text = stone.ToString();
                    return true;
                }
                break;
            case ResourceType.Food:
                if (food >= amount)
                {
                    food -= amount;
                    textFood.text = food.ToString();
                    return true;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
        print("Not enough " + type);
        return false;
    }
}

public enum ResourceType
{
    Wood,
    Stone,
    Food
}
