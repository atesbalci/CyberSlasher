namespace Game
{
    public class Minion : Enemy
    {
        protected override float Damage
        {
            get { return 0.2f; }
        }

        public override void Hit(HitType hitType)
        {
            IsDead = true;
        }
    }
}
