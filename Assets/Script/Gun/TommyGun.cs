using System.Collections;

namespace Script
{
    public class TommyGun : Gun
    {
        protected override IEnumerator ShootRecoil()
        {
            yield return base.ShootRecoil(recoilAngle/2, recoilTime/5f);
            yield return base.ShootRecoil(-recoilAngle/2, recoilTime/5f);
            yield return base.ShootRecoil(recoilAngle/2, recoilTime/5f);
            yield return base.ShootRecoil(-recoilAngle/2, recoilTime/5f);
            yield return base.ShootRecoil(recoilAngle, recoilTime/5f);
        }
    }
}