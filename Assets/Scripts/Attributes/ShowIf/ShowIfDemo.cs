using Attributes.NonSerializedField;
using UnityEngine;

namespace Attributes.ShowIf
{
    public class ShowIfDemo : MonoBehaviour
    {
        [SerializeField] private bool _showIf;
        [SerializeField, ShowIf("_showIf")] private string _showIfString;
        [NonSerializedField, SerializeField]private string _readonly;
    }
}
