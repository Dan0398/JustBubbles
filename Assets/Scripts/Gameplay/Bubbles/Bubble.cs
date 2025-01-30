using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    [System.Serializable]
    public class Bubble :  Gameplay.User.ICircleObject
    {
        [System.Serializable]
        public enum BubbleColor
        {
            Red, Green, Yellow, Purple, Cyan, Orange
        }
        
        [field:SerializeField] public Transform MyTransform { get; private set; }
        [field:SerializeField] public Rigidbody2D MyRigid   { get; private set; }
        [field:SerializeField] public GameObject OnScene    { get; private set; }
        [field:SerializeField] public BubbleColor MyColor   { get; private set; }
        [field:SerializeField] public Vector3 LocalPosInLine{ get; private set; }
        public System.Action OnSceneAnimationEnds;
        Collider2D Collisions;
        float Size;
        
        public System.Func<Color> TrajectoryColor => () => Gameplay.ColorPicker.GetColorByEnum(MyColor);
        
        public Bubble(GameObject Sample, Content.Bubble BubblesSkins)
        {
            OnScene = GameObject.Instantiate(Sample);
            OnScene.SetActive(true);
            MyTransform = OnScene.transform;
            Collisions = OnScene.GetComponent<Collider2D>();
            MyRigid = OnScene.AddComponent<Rigidbody2D>();
            MyRigid.isKinematic = true;
            if (BubblesSkins != null)
            {
                var View = OnScene.GetComponent<SpriteRenderer>();
                View.sharedMaterial = BubblesSkins.SelectedContent.Value;
                BubblesSkins.SelectedContent.Changed += () => View.sharedMaterial = BubblesSkins.SelectedContent.Value;
            }
        }
        
        public void ChangeColor(BubbleColor NewColor)
        {
            MyColor = NewColor;
            OnScene.GetComponent<SpriteRenderer>().color = ColorPicker.GetColorByEnum(MyColor);
        }
        
        public void SetSize(float BubbleSize)
        {
            Size = BubbleSize;
            MyTransform.localScale = Vector3.one * BubbleSize;
        }
        
        public void RandomizeColor(int MaxColorID, float RandomizeFactor = 1) => ChangeColor(ColorPicker.GetRandomColor(MaxColorID, RandomizeFactor));
        public void RandomizeColor(List<BubbleColor> AvailableColors, float RandomizeFactor = 1) => ChangeColor(ColorPicker.GetRandomColor(AvailableColors, RandomizeFactor));
        
        public void PlaceInLine(Transform LineTransform, int IdOnLine)
        {
            MyTransform.SetParent(LineTransform);
            LocalPosInLine = Vector3.right * Size * IdOnLine;
            MyTransform.localPosition = LocalPosInLine;
            MyTransform.localRotation = Quaternion.identity;
        }
        
        public void DeactivateCollisions() => Collisions.enabled = false;
        
        public void ActivateCollisions() => Collisions.enabled = true;
    }
}