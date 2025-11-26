using System.Collections;
using UnityEngine;

namespace Calendar.Scripts.Services.CoroutineRunner
{
    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator routine);
    }
}