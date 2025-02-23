using UnityEngine;

namespace Gameplay.User
{
    [System.Serializable]
    public class RayTrajectory
    {
        [Header("Line info"), SerializeField, Min(0.05f)] private float _spaceBetweenDrawn;
        [SerializeField] private float _meshSize = 0.5f;
        [SerializeField, Range(0.001f, 0.03f)] private float _animationSpeed;
        [SerializeField] private Mesh _drawnMesh;
        [SerializeField] private Material _drawnMaterial;
        
        private Vector3 _parentPos;
        private Matrix4x4[] _points;
        private float _drawnShift;
        private int _drawnPointsCount;
        private int _colorNameIDInShader;
        private Color _trajectoryBaseColor;
        private float _oldAlpha;
        private int _collisionLayer;
        private Trajectory _trajectory;
        private System.Func<Collider2D,CollisionType> _responser;
        
        public void Init(Gameplay.User.Action parent, System.Func<Collider2D,CollisionType> TryResponseCollision)
        {
            _parentPos = parent.transform.position;
            _points = new Matrix4x4[256];
            _collisionLayer = 1;
            _colorNameIDInShader = Shader.PropertyToID("_MaskColor");
            _responser = TryResponseCollision;
        }
        
        public void RefreshConfig(float Radius, int CollisionsCount = 20, float Distance = float.MaxValue)
        {
            _trajectory = new Trajectory(Radius, _responser, _collisionLayer, CollisionsCount, Distance);
        }
        
        public void ProcessRay(Vector2 MouseDirection)
        {
            _trajectory.CalculateFullWayClean(_parentPos, MouseDirection);
        }
        
        public void ReceiveAvailableAndTryDraw(bool allowed)
        {
            const float Step = 0.05f;
            var Alpha = Mathf.Clamp01(_oldAlpha + Step * (allowed? 1 : (-1)));
            if (Alpha == 0) return;
            if (Alpha != _oldAlpha)
            {
                _oldAlpha = Alpha;
                RefreshFinalColor();
            }
            if(allowed)
            {
                Draw();
            }
        }
        
        private void Draw()
        {
            RefreshMatrices();
            Graphics.DrawMeshInstanced(_drawnMesh, 0, _drawnMaterial, _points, _drawnPointsCount);
            _drawnShift += _animationSpeed;
            if (_drawnShift >= _spaceBetweenDrawn)
            {
                _drawnShift -= _spaceBetweenDrawn;
            }
        }
        
        private void RefreshMatrices()
        {
            _trajectory.ResetWay();
            _trajectory.MakeStep(_drawnShift);
            _trajectory.StepLengthOnWay = _spaceBetweenDrawn;
            _drawnPointsCount = 0;
            Vector3 Scale = Vector3.one * _meshSize;
            Vector3 PosFix = Vector3.back * 0.1f;
            while (!_trajectory.WayEnded && _drawnPointsCount < 255)
            {
                _points[_drawnPointsCount] = Matrix4x4.TRS(_trajectory.PosOnWay + PosFix, Quaternion.identity, Scale);
                _drawnPointsCount++;
                _trajectory.MakeStep();
            }
        }
        
        public void ChangeColor(Color Col)
        {
            if (Col == _trajectoryBaseColor) return;
            _trajectoryBaseColor = Col;
            RefreshFinalColor();
             
        }
        
        private void RefreshFinalColor()
        {
            _drawnMaterial.SetColor(_colorNameIDInShader, _trajectoryBaseColor - Color.black * (1-_oldAlpha));
        }
    }
}