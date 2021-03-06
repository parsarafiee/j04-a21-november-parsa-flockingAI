using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MOve : MonoBehaviour
{
    Rigidbody2D m_Rigidbody2;

    public float xBorder;
    public float yBorder;  

    public Slider  alignmentWeight;


    public float MaxSpeed;
    public float MaxAcceleration;


    public float cohesionAreaRed = 10;
    Collider2D[] allBirds;

    Vector3 target;
    public Vector3 initialVlocity;

    float dir;
    public float alignmentAreaRed = 5;

    public float seperationAreaRed = 1;


    List<GameObject> attractionArea;
    List<GameObject> alignmentArea;
    List<GameObject> seperationArea;

    float timer;


    // Start is called before the first frame update
    void Start()
    {
        attractionArea = new List<GameObject>();
        alignmentArea = new List<GameObject>();
        seperationArea = new List<GameObject>();

        m_Rigidbody2 = gameObject.GetComponent<Rigidbody2D>();
        m_Rigidbody2.velocity = initialVlocity;

        float angle = Mathf.Atan2(initialVlocity.y, initialVlocity.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (this.transform.position.x > xBorder)
        {
            transform.position = new Vector3(-xBorder, transform.position.y, 0);
        }
        if (this.transform.position.x < -xBorder)
        {
            transform.position = new Vector3(xBorder, transform.position.y, 0);
        }
        if (this.transform.position.y > yBorder)
        {
            transform.position = new Vector3(transform.position.x, -yBorder, 0);
        }
        if (this.transform.position.y < -yBorder)
        {
            transform.position = new Vector3(transform.position.x, yBorder, 0);
        }
        allBirds = Physics2D.OverlapCircleAll(transform.position, cohesionAreaRed);
        PutInCorrectList();



    }

    void FixedUpdate()
    {
        Vector2 sum=Vector2.zero;
        if (attractionArea.Count > 0)
        {
            target = GetDestination(attractionArea);
            Vector2 targetSpeed = (target - this.transform.position).normalized * MaxSpeed;
            sum+= ((targetSpeed - m_Rigidbody2.velocity).normalized * Time.deltaTime * MaxAcceleration);
        }


        if (seperationArea.Count > 0)
        {

            sum += (Vector2)SeprationForce();
        }

        if (alignmentArea.Count > 0)
        {
            Vector2 newVelo = SetOrientation(alignmentArea);
            sum +=(((newVelo.normalized) * MaxSpeed - m_Rigidbody2.velocity).normalized * Time.deltaTime * MaxAcceleration);
        }

        m_Rigidbody2.AddForce(sum, ForceMode2D.Impulse);
        float angle1 = Mathf.Atan2(m_Rigidbody2.velocity.y, m_Rigidbody2.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle1, Vector3.forward);

    }
    //void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, cohesionAreaRed);
    //}

    Vector3 SeprationForce()
    {
        Vector3 seprationForceDIrection = Vector3.zero;
        for (int i = 0; i < seperationArea.Count; i++)
        {
            seprationForceDIrection += (this.transform.position - seperationArea[i].gameObject.transform.position) / Vector3.Distance(this.transform.position, seperationArea[i].gameObject.transform.position);
        }
        seprationForceDIrection = (seprationForceDIrection / seperationArea.Count).normalized * MaxSpeed;
        return (((Vector2)seprationForceDIrection - m_Rigidbody2.velocity).normalized * Time.deltaTime * MaxAcceleration); ;
    }

    Vector3 GetDestination(List<GameObject> list)
    {
        Vector3 pos = Vector3.zero;
        int number = 0;

        if (list.Count > 0)
        {
            for (int i = 0; i < list.Count; i++)
            {
                number++;
                pos += list[i].gameObject.transform.position;
            }
            return pos / number;

        }
        return pos;
    }

    void PutInCorrectList()
    {
        attractionArea.Clear();
        alignmentArea.Clear();
        seperationArea.Clear();
        for (int i = 0; i < allBirds.Length; i++)
        {
            if (allBirds[i].gameObject != this.gameObject)
            {
                float distance = Vector3.Distance(this.transform.position, allBirds[i].gameObject.transform.position);

                if (distance < seperationAreaRed)
                {
                    seperationArea.Add(allBirds[i].gameObject);
                }
                else if (distance < alignmentAreaRed)
                {
                    alignmentArea.Add(allBirds[i].gameObject);
                }
                else if (distance < cohesionAreaRed)
                {
                    attractionArea.Add(allBirds[i].gameObject);
                }

            }

        }

    }
    private Vector2 SetOrientation(List<GameObject> list)
    {
        Vector2 vlo = Vector2.zero;
        if (list.Count > 0)
        {
            for (int i = 0; i < list.Count; i++)
            {
                vlo += list[i].GetComponent<Rigidbody2D>().velocity;
            }

            vlo /= list.Count;
        }
        return vlo;
    }
}
