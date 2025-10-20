using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Witmina_SpaceBurgers
{
    public enum FoodType
    {
        Burger,
        Fries,
        Drink,
    }
    public enum IngredientType
    {
        BreadBottom,
        BreadTop,
        Beef,
        Chicken,
        Cheddar,
        Lettuce,
        Tomato,
        Onion,
        Fries,
        Ketchup,
        Mustard,
        Red_Soda,
        Green_Soda,
        Blue_Soda,
    }

    public enum AudioFxType
    {
        OrderGive,
        SelectIngredient,
        OrderServe,
        RefillStart,
        RefillComplete,
        OrderFail,
        OrderWin,
        OrderWin2,
        BurgerPlate,
        FriesPour,
        SauceSplash,
        DrinkPour,
        TrashThrow,
        TrashOpen,
        TrashClose,
        IntroPanelOpen,
        IntroPanelClose,
    }
}