using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML
{
    [Serializable]
    public class Features
    {

        private bool _useMomentum = false;

        private bool _useAdaGrad = false;

        private bool _useRMSProp = false;

        private bool _useAdam = false;

        // doesn't dropout input nodes
        private bool _useRegularDropOut = false;

        // doesn't dropout input nodes
        private bool _useDynamicDropOut = false;

        public bool UseAdam
        {
            get { return _useAdam; }
            set
            {
                if (value)
                {
                    _useMomentum = false;
                    _useAdaGrad = false;
                    _useRMSProp = false;
                }
                _useAdam = value;
            }
        }

        public bool UseRMSProp
        {
            get { return _useRMSProp; }
            set
            {
                if (value)
                {
                    _useAdam = false;
                }
                _useRMSProp = value;
            }
        }

        public bool UseAdaGrad
        {
            get { return _useAdaGrad; }
            set
            {
                if (value)
                {
                    _useAdam = false;
                }
                _useAdaGrad = value;
            }
        }

        public bool UseMomentum
        {
            get { return _useMomentum; }
            set
            {
                if (value)
                {
                    _useAdam = false;
                }
                _useMomentum = value;
            }
        }

        public bool UseRegularDropOut
        {
            get { return _useRegularDropOut; }
            set
            {
                if (value)
                {
                    _useDynamicDropOut = false;
                }
                _useRegularDropOut = value;
            }
        }

        public bool UseDynamicDropOut
        {
            get { return _useDynamicDropOut; }
            set
            {
                if (value)
                {
                    _useRegularDropOut = false;
                }
                _useDynamicDropOut = value;
            }
        }

        public Features()
        {

        }
    }
}
