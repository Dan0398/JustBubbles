using UnityEngine;

namespace Gameplay.User
{
    [System.Serializable]
    public class RayTrajectory
    {
        [Header("Line info"), SerializeField, Min(0.05f)] float SpaceBetweenDrawn;
        [SerializeField] float MeshSize = 0.5f;
        [SerializeField, Range(0.001f, 0.03f)] float AnimationSpeed;
        [SerializeField] Mesh DrawnMesh;
        [SerializeField] Material DrawnMaterial;
        
        Gameplay.User.Action parent;
        Vector3 parentPos;
        Matrix4x4[] points;
        float drawnShift;
        int drawnPointsCount;
        int colorNameIDInShader;
        Color TrajectoryBaseColor;
        float oldAlpha;
        int CollisionLayer;
        Trajectory trajectory;
        System.Func<Collider2D,CollisionType> Responser;
        
        public void Init(Gameplay.User.Action parent/*, float BubbleCollisionRadius*/, System.Func<Collider2D,CollisionType> TryResponseCollision)
        {
            parentPos = parent.transform.position;
            this.parent = parent;
            points = new Matrix4x4[256];
            CollisionLayer = 1;
            colorNameIDInShader = Shader.PropertyToID("_MaskColor");
            Responser = TryResponseCollision;
        }
        
        public void RefreshConfig(float Radius, int CollisionsCount = 20, float Distance = float.MaxValue)
        {
            trajectory = new Trajectory(Radius, Responser, CollisionLayer, CollisionsCount, Distance);
        }
        
        public void ProcessRay(Vector2 MouseDirection)
        {
            trajectory.CalculateFullWayClean(parentPos, MouseDirection);
        }
        
        internal void ReceiveAvailableAndTryDraw(bool allowed)
        {
            const float Step = 0.05f;
            var Alpha = Mathf.Clamp01(oldAlpha + Step * (allowed? 1 : (-1)));
            if (Alpha == 0) return;
            if (Alpha != oldAlpha)
            {
                oldAlpha = Alpha;
                RefreshFinalColor();
            }
            if(allowed) Draw();
        }
        
        void Draw()
        {
            RefreshMatrices();
            Graphics.DrawMeshInstanced(DrawnMesh, 0, DrawnMaterial, points, drawnPointsCount);
            drawnShift += AnimationSpeed;
            if (drawnShift >= SpaceBetweenDrawn)
            {
                drawnShift -= SpaceBetweenDrawn;
            }
        }
        
        void RefreshMatrices()
        {
            trajectory.ResetWay();
            trajectory.MakeStep(drawnShift);
            trajectory.StepLengthOnWay = SpaceBetweenDrawn;
            drawnPointsCount = 0;
            Vector3 Scale = Vector3.one * MeshSize;
            Vector3 PosFix = Vector3.back * 0.1f;
            while (!trajectory.WayEnded && drawnPointsCount < 255)
            {
                points[drawnPointsCount] = Matrix4x4.TRS(trajectory.PosOnWay + PosFix, Quaternion.identity, Scale);
                drawnPointsCount++;
                trajectory.MakeStep();
            }
        }
        
        public void ChangeColor(Color Col)
        {
            if (Col == TrajectoryBaseColor) return;
            TrajectoryBaseColor = Col;
            RefreshFinalColor();
             
        }
        
        void RefreshFinalColor() => DrawnMaterial.SetColor(colorNameIDInShader, TrajectoryBaseColor - Color.black * (1-oldAlpha));
    }
}