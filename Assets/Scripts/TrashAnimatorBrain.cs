using System.Collections;
using UnityEngine;

public class TrashAnimatorBrain : MonoBehaviour
{
    public enum Animations
    {
        IDLE,
        WALK,
        JUMP,
        HOWL,
        RESET
    }

    [System.Serializable]
    public class AnimationData
    {
        public Animations animation;
        public float crossfade;
        public bool lockLayer;
        public AnimationData nextAnimation;

        public AnimationData(Animations animation, float crossfade = 0.1f, bool lockLayer = false, AnimationData nextAnimation = null)
        {
            this.animation = animation;
            this.crossfade = crossfade;
            this.lockLayer = lockLayer;
            this.nextAnimation = nextAnimation;
        }
    }

    private Animator animator;
    private Animations[] currentAnimation;
    private bool[] layerLocked;
    private Coroutine[] currentCoroutine;
    private static readonly int[] animationHashes = InitHashes();

    private void Awake()
    {
        animator = GetComponent<Animator>();
        int layers = animator.layerCount;
        currentAnimation = new Animations[layers];
        layerLocked = new bool[layers];
        currentCoroutine = new Coroutine[layers];

        for (int i = 0; i < layers; i++)
        {
            currentAnimation[i] = Animations.RESET;
            layerLocked[i] = false;
        }
    }

    public virtual void DefaultAnimation(int layer)
    {
        Play(new AnimationData(Animations.IDLE), layer);
    }

    public bool Play(AnimationData data, int layer = 0)
    {
        try
        {
            if (data.animation == Animations.RESET)
            {
                DefaultAnimation(layer);
                return false;
            }

            if (layerLocked[layer] || currentAnimation[layer] == data.animation)
                return false;

            if (currentCoroutine[layer] != null)
                StopCoroutine(currentCoroutine[layer]);

            layerLocked[layer] = data.lockLayer;
            currentAnimation[layer] = data.animation;
            
            
            Debug.Log($"Trying to play {data.animation} | Hash: {animationHashes[(int)data.animation]}");

            animator.CrossFade(animationHashes[(int)currentAnimation[layer]], data.crossfade, layer);

            if (data.nextAnimation != null)
            {
                currentCoroutine[layer] = StartCoroutine(ChainCoroutine(data.nextAnimation, layer));
            }

            return true;
        }
        catch
        {
            Debug.LogError($"[TrashAnimatorBrain] Failed to play animation on layer {layer}. Make sure Animator is set up correctly.");
            return false;
        }
    }

    private IEnumerator ChainCoroutine(AnimationData nextData, int layer)
    {
        animator.Update(0);
        float delay = animator.GetNextAnimatorStateInfo(layer).length;
        if (nextData.crossfade == 0)
            delay = animator.GetCurrentAnimatorStateInfo(layer).length;

        yield return new WaitForSeconds(delay - nextData.crossfade);
        layerLocked[layer] = false;
        Play(nextData, layer);
    }

    private static int[] InitHashes()
    {
        Animations[] values = (Animations[])System.Enum.GetValues(typeof(Animations));
        int[] hashes = new int[values.Length];
        for (int i = 0; i < values.Length; i++)
            hashes[i] = Animator.StringToHash(values[i].ToString());
        return hashes;
    }
}
