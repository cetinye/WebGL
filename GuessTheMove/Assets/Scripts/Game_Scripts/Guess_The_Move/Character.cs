using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Guess_The_Move
{
    public class Character : MonoBehaviour
    {
        public SkinnedMeshRenderer meshRenderer;
        public int hairMaterialIndex;
        public int shoesMaterialIndex;
        public int shirtMaterialIndex;
        public int pantMaterialIndex;

        [Header("Attributes")] public CharacterType charType;

        public Animator animator;
        public Dances dance;

        [Header("Flags")] public bool isHairRandomized;

        public bool isShoesRandomized;
        public bool isShirtRandomized;
        public bool isPantRandomized;
        public bool isDanceRandomized;
        public bool isOutfitRandomized;

        private void Awake()
        {
            meshRenderer.materials[hairMaterialIndex] = Instantiate(meshRenderer.materials[hairMaterialIndex]);
            meshRenderer.materials[shoesMaterialIndex] = Instantiate(meshRenderer.materials[shoesMaterialIndex]);
            meshRenderer.materials[shirtMaterialIndex] = Instantiate(meshRenderer.materials[shirtMaterialIndex]);
            meshRenderer.materials[pantMaterialIndex] = Instantiate(meshRenderer.materials[pantMaterialIndex]);

            for (var i = 0; i < Enum.GetValues(typeof(Dances)).Length; i++)
            {
                var danceStr = ((Dances)i).ToString();

                if (danceStr.Equals(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name))
                    dance = (Dances)i;
            }
        }

        public void InitialRandomize()
        {
            meshRenderer.materials[hairMaterialIndex].color =
                LevelManager.instance.colors[Random.Range(0, LevelManager.instance.colors.Count)];
            meshRenderer.materials[shoesMaterialIndex].color =
                LevelManager.instance.colors[Random.Range(0, LevelManager.instance.colors.Count)];
            meshRenderer.materials[shirtMaterialIndex].color =
                LevelManager.instance.colors[Random.Range(0, LevelManager.instance.colors.Count)];
            meshRenderer.materials[pantMaterialIndex].color =
                LevelManager.instance.colors[Random.Range(0, LevelManager.instance.colors.Count)];
        }

        public void Randomize()
        {
            var characterOld = LevelManager.instance.characterOld;

            //Hair
            if (isHairRandomized)
                do
                {
                    meshRenderer.materials[hairMaterialIndex].color =
                        LevelManager.instance.colors[Random.Range(0, LevelManager.instance.colors.Count)];
                } while (meshRenderer.materials[hairMaterialIndex].color
                         .Equals(characterOld.meshRenderer.materials[characterOld.hairMaterialIndex].color));

            //Shoes
            if (isShoesRandomized)
                do
                {
                    meshRenderer.materials[shoesMaterialIndex].color =
                        LevelManager.instance.colors[Random.Range(0, LevelManager.instance.colors.Count)];
                } while (meshRenderer.materials[shoesMaterialIndex].color
                         .Equals(characterOld.meshRenderer.materials[characterOld.shoesMaterialIndex].color));

            //Shirt
            if (isShirtRandomized)
                do
                {
                    meshRenderer.materials[shirtMaterialIndex].color =
                        LevelManager.instance.colors[Random.Range(0, LevelManager.instance.colors.Count)];
                } while (meshRenderer.materials[shirtMaterialIndex].color
                         .Equals(characterOld.meshRenderer.materials[characterOld.shirtMaterialIndex].color));

            //Pant
            if (isPantRandomized)
                do
                {
                    meshRenderer.materials[pantMaterialIndex].color =
                        LevelManager.instance.colors[Random.Range(0, LevelManager.instance.colors.Count)];
                } while (meshRenderer.materials[pantMaterialIndex].color
                         .Equals(characterOld.meshRenderer.materials[characterOld.pantMaterialIndex].color));

            //Dance
            if (isDanceRandomized)
                do
                {
                    var randDance = (Dances)Random.Range(0, (int)Enum.GetValues(typeof(Dances)).Cast<Dances>().Max());
                    dance = randDance;
                    animator.SetTrigger(randDance.ToString());
                } while (dance.Equals(LevelManager.instance.characterOld.dance));

            //Outfit
            // if (isOutfitRandomized)
            // {
            //     do
            //     {
            //         characters[UnityEngine.Random.Range(0, characters.Count)]

            //     } while (dance.Equals(LevelManager.instance.characterOld.dance));
            // }
        }

        public void SetVisibility(bool state)
        {
            meshRenderer.enabled = state;
            animator.enabled = state;
        }
    }

    public enum CharacterType
    {
        Char1,
        Char2,
        Char3,
        Char4,
        Char5,
        Char6
    }

    public enum Dances
    {
        HipHop,
        Salsa,
        Silly,
        Swing
    }
}