namespace Game
{
    public class Boss : Enemy
    {
        public override void Hit(HitType hitType)
        {
            if (hitType == HitType.Super)
            {
                Destroy(gameObject);
            }
        }
    }
}
