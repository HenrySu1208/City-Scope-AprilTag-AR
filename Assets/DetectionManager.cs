using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI = UnityEngine.UI;
using Klak.TestTools;

sealed class DetectionManager : MonoBehaviour
{
    [SerializeField] ImageSource _source = null;
    [SerializeField] int _decimation = 4;
    [SerializeField] float _tagSize = 0.05f;
    [SerializeField] Material _tagMaterial = null;
    [SerializeField] UI.RawImage _webcamPreview = null;
    [SerializeField] UI.Text _debugText = null;
    [SerializeField] GameObject ModelDisplay = null;

    AprilTag.TagDetector _detector;
    TagDrawer _drawer;
    ModelDrawer _ModelDrawer;
    Vector3 temp;
    public List<AprilTag.TagPose> TagList = new List<AprilTag.TagPose>();

    void Start()
    {
        var dims = _source.OutputResolution;
        _detector = new AprilTag.TagDetector(dims.x, dims.y, _decimation);
        _drawer = new TagDrawer(_tagMaterial);
    }

    // void OnDestroy()
    // {
    //     _detector.Dispose();
    //     _drawer.Dispose();
    // }

    void LateUpdate()
    {
        _webcamPreview.texture = _source.Texture;

        // Source image acquisition
        var image = _source.Texture.AsSpan();
        if (image.IsEmpty) return;

        // AprilTag detection
        var fov = Camera.main.fieldOfView * Mathf.Deg2Rad;
        _detector.ProcessImage(image, fov, _tagSize);

        // Detected tag visualization
        DetectTag();

        if (TagList.Count > 0)
        {
            temp = TagList[0].Position;
            //temp[0] += 0.05f;
            float x = TagList[0].Rotation[0];
            float y = TagList[0].Rotation[1];
            float z = TagList[0].Rotation[2];
            float w = TagList[0].Rotation[3];
            float t = 2 * (w * y - z * w);
            if (Mathf.Abs(t) > 1.0)
            {
                t = t / Mathf.Abs(t);
            }
            double x_angle = Mathf.Atan2((2 * (w * x + y * z)), (1 - 2 * (x * x + y * y)));
            double y_angle = Mathf.Asin(t);
            double z_angle = Mathf.Atan2((2 * (w * z + x * y)), (1 - 2 * (z * z + y * y)));

            temp[2] += 0.05f;
            // temp[0] += 0.05f * Mathf.Cos(120 * Mathf.Deg2Rad) * Mathf.Cos(-40 * Mathf.Deg2Rad);
            // temp[1] += 0.05f * Mathf.Cos(120 * Mathf.Deg2Rad) * Mathf.Sin(-40 * Mathf.Deg2Rad);
            // temp[2] += 0.05f * Mathf.Sin(120 * Mathf.Deg2Rad);
            //_drawer.Draw(TagList[0].ID, TagList[0].Position, TagList[0].Rotation, _tagSize);
            _ModelDrawer = new ModelDrawer(ModelDisplay, temp, TagList[0].Rotation, 100f);
            Debug.Log(TagList[0].Position);
            Debug.Log(TagList[0].Rotation);

        }
        TagList.Clear();

        // Profile data output (with 30 frame interval)
        if (Time.frameCount % 30 == 0)
            _debugText.text = _detector.ProfileData.Aggregate
              ("Profile (usec)", (c, n) => $"{c}\n{n.name} : {n.time}");

    }

    void DetectTag()
    {
        foreach (var tag in _detector.DetectedTags)
        {
            CreateList(tag);
        }
    }

    void CreateList(AprilTag.TagPose tag)
    {
        TagList.Add(tag);
    }
}

// void SortTags(int[] tags)
// {
//     for (int i = 0; i < tags.Length - 1; i++)
//     {
//         int minimumTag = i;

//         for (int j = i + 1; j < tags.Length; j++)
//         {
//             if (tags[j] < tags[minimumTag]) minimumTag = j;
//         }

//         if (minimumTag != i)
//         {
//             int temp = tags[i];
//             tags[i] = tags[minimumTag];
//             tags[minimumTag] = temp;
//         }
//     }
// }
