namespace Game
{
    public class Minion : Enemy
    {
        public override void Hit(HitType hitType)
        {
            Destroy(gameObject);
        }
    }
}
