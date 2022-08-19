using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class BottleController : MonoBehaviour
{
   public Color []bottleColors;
   
   public SpriteRenderer bottleMaskSR;

   public AnimationCurve ScaleAndRotationMultiplierCurve;
   public AnimationCurve FillAmountCurve;
   public AnimationCurve RotationSpeedMultiplier;

   public float timeToRotate = 1f;

   private void Start()
   {
      UpdateColorsOnShader();
   }

   private void Update()
   {
      if (Input.GetKeyDown(KeyCode.P))
      {
         StartCoroutine(RotateBottle());
      }
   }

   private void UpdateColorsOnShader()
   {
      bottleMaskSR.material.SetColor("_C1",bottleColors[0]);
      bottleMaskSR.material.SetColor("_C2",bottleColors[1]);
      bottleMaskSR.material.SetColor("_C3",bottleColors[2]);
      bottleMaskSR.material.SetColor("_C4",bottleColors[3]);
   }

   private IEnumerator RotateBottle()
   {
      float t = 0;
      float lerpValue;
      float angleValue;

      while (t < timeToRotate)
      {
         lerpValue = t / timeToRotate;
         angleValue = Mathf.Lerp(0f, 90f, lerpValue);

         transform.eulerAngles = new Vector3(0, 0, angleValue);
         bottleMaskSR.material.SetFloat("_SARM",ScaleAndRotationMultiplierCurve.Evaluate(angleValue));
         bottleMaskSR.material.SetFloat("_FillAmount",FillAmountCurve.Evaluate(angleValue));

         t += Time.deltaTime * RotationSpeedMultiplier.Evaluate(angleValue);

         yield return new WaitForEndOfFrame();
      }
      angleValue = 90;
      transform.eulerAngles = new Vector3(0, 0, angleValue);
      bottleMaskSR.material.SetFloat("_SARM",ScaleAndRotationMultiplierCurve.Evaluate(angleValue));
      bottleMaskSR.material.SetFloat("_FillAmount",FillAmountCurve.Evaluate(angleValue));

      StartCoroutine(RotateBottleBack());
   }

   private IEnumerator RotateBottleBack()
   {
      float t = 0;
      float lerpValue;
      float angleValue;

      while (t < timeToRotate)
      {
         lerpValue = t / timeToRotate;
         angleValue = Mathf.Lerp(90f, 0f, lerpValue);

         transform.eulerAngles = new Vector3(0, 0, angleValue);
         bottleMaskSR.material.SetFloat("_SARM",ScaleAndRotationMultiplierCurve.Evaluate(angleValue));

         t += Time.deltaTime * 10;

         yield return new WaitForEndOfFrame();
      }
      angleValue = 0;
      transform.eulerAngles = new Vector3(0, 0, angleValue);
      bottleMaskSR.material.SetFloat("_SARM",ScaleAndRotationMultiplierCurve.Evaluate(angleValue));
   }
}
