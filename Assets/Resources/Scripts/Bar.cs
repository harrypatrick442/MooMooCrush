using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    public class Bar : MonoBehaviour
    {
        public Health Health = new Health();
        private HealthEventHandler _HealthEventHandler;
        private Slider _Slider;
        public float StartProportion = 1;
        private Slider Slider
        {
            get
            {
                if (_Slider == null)
                {
                    _Slider = transform.Find("Bar").GetComponent<Slider>();
                }
                return _Slider;
            }
        }
        private Image _Image;
       private Image Image
        {
            get
            {
                if (_Image == null)
                    _Image = gameObject.transform.Find("Bar").Find("Fill Area").Find("Fill").GetComponent<Image>();
                return _Image;
            }
        }
        private void HealthChanged(HealthEventArgs e)
        {
            Slider.value = Health.Proportion;
            Image.color = new Color(1 - Health.Proportion, Health.Proportion, 0);
        }
        private void Start()
        {
            _HealthEventHandler = new HealthEventHandler(HealthChanged);
            Health.AddEventHandler(_HealthEventHandler);
            Health.Proportion = StartProportion;
        }
    }
}
