using System;
using Ghost.Xenus.Protocol.Events.Data;

namespace Ghost.Xenus
{
    public class CaptchaEventArgs : EventArgs
    {
        public Captcha Captcha { get; }

        internal CaptchaEventArgs(Captcha captcha)
        {
            Captcha = captcha;
        }
    }
}