using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JacobianIK : MonoBehaviour
{
    public GameObject target;
    public GameObject endEffector;
    public float positionAlpha = 10f;
    public float rotationAlpha = 5f;
    
    private ArticulationBody arBody;
    
    private ArticulationJacobian arJacobian;

    private List<float> jointSpacePositions;

    private List<float> jointSpaceTargets;
    private List<int> jointDofStarts;
    
    
    // Start is called before the first frame update
    void Start()
    {
        arBody = gameObject.GetComponent<ArticulationBody>();
        arJacobian = new ArticulationJacobian();
        arJacobian.elements = new List<float>();
        jointSpacePositions = new List<float>();
        jointSpaceTargets = new List<float>();
        jointDofStarts = new List<int>();
    }

    
    List<float> JacobianMultiply(ArticulationJacobian jacobian, List<float> targetDelta)
    {
        List<float> result = new List<float>(jacobian.rows);
        for (int i = 0; i < jacobian.rows; i++)
        {
            result.Add(0.0f);
            for (int j = 0; j < jacobian.columns; j++)
            {
                result[i] += jacobian[i, j] * targetDelta[j];
            }
        }

        return result;
    }

    ArticulationJacobian JacobianMultiply(ArticulationJacobian jacobian1, ArticulationJacobian jacobian2)
    {
        if (jacobian1.columns != jacobian2.rows)
            throw new Exception("Can't multiply jacobians, jacobian1.columns != jacobian2.rows!");
        ArticulationJacobian result = new ArticulationJacobian(jacobian1.rows, jacobian2.columns);
        for (int row = 0; row < jacobian1.rows; row++)
            for (int column = 0; column < jacobian2.columns; column++)
                for (int i = 0; i < jacobian1.columns; i++)
                    result[row, column] += jacobian1[row, i] * jacobian2[i, column];
        return result;
    }
    
    ArticulationJacobian JacobianMultiply(ArticulationJacobian jacobian, float value)
    {
        ArticulationJacobian result = new ArticulationJacobian(jacobian.rows, jacobian.columns);
        for (int row = 0; row < jacobian.rows; row++)
            for (int column = 0; column < jacobian.columns; column++)
                result[row, column] = jacobian[row, column] * value;
        return result;
    }

    ArticulationJacobian JacobianAdd(ArticulationJacobian jacobian1, ArticulationJacobian jacobian2)
    {
        if (jacobian1.rows != jacobian2.rows || jacobian1.columns != jacobian2.columns)
            throw new Exception("Can't add jacobians, matrix dimensions are not equal!");
        ArticulationJacobian result = new ArticulationJacobian(jacobian1.rows, jacobian1.columns);
        for (int row = 0; row < jacobian1.rows; row++)
            for (int column = 0; column < jacobian1.columns; column++)
                result[row, column] = jacobian1[row, column] + jacobian2[row, column];
        return result;
    }
    
    ArticulationJacobian JacobianTranspose(ArticulationJacobian jacobian)
    {
        ArticulationJacobian jacobianT = new ArticulationJacobian(jacobian.columns, jacobian.rows);
        for (int i = 0; i < jacobian.rows; i++)
        {
            for (int j = 0; j < jacobian.columns; j++)
                jacobianT[j, i] = jacobian[i, j];
        }

        return jacobianT;
    }

    void JacobianSwapRows(ArticulationJacobian jacobian, int row1, int row2)
    {
        if (row1 == row2)
            return;
        for (int i = 0; i < jacobian.columns; i++)
        {
            float temp = jacobian[row1, i];
            jacobian[row1, i] = jacobian[row2, i];
            jacobian[row2, i] = temp;
        }
    }
    ArticulationJacobian JacobianInverse(ArticulationJacobian jacobian)
    {
        const float deltaE = 1e-8f;
        if (jacobian.rows != jacobian.columns)
            throw new Exception("Can't find inverse for non square rows != columns jacobian!");
        ArticulationJacobian jacobianInv = new ArticulationJacobian(jacobian.rows, jacobian.columns);
        // Initialize to identity
        for (int diagonal = 0; diagonal < jacobianInv.rows; diagonal++)
            jacobianInv[diagonal, diagonal] = 1.0f;
        
        for (int diagonal = 0; diagonal < jacobian.rows; diagonal++)
        {
            int maxRow = diagonal;
            float maxValue = Math.Abs(jacobian[diagonal, diagonal]);
            float currentValue;
            for (int row = diagonal + 1; row < jacobian.rows; row++)
            {
                currentValue = Math.Abs(jacobian[row, diagonal]);
                if (currentValue > maxValue)
                {
                    maxRow = row;
                    maxValue = currentValue;
                }
            }
            if (maxValue < deltaE)
                throw new Exception("Jacobian is degenerate, can't compute inverse!");
            JacobianSwapRows(jacobian, diagonal, maxRow);
            JacobianSwapRows(jacobianInv, diagonal, maxRow);


            float inverseElement = 1 / jacobian[diagonal, diagonal];
            for (int col = diagonal; col < jacobian.columns; col++)
                jacobian[diagonal, col] *= inverseElement;
            for (int col = 0; col < jacobianInv.columns; col++)
                jacobianInv[diagonal, col] *= inverseElement;
            for (int row = 0; row < jacobian.rows; row++)
            {
                float value = jacobian[row, diagonal];
                if (row != diagonal)
                {
                    for (int column = diagonal; column < jacobian.columns; column++)
                        jacobian[row, column] -= value * jacobian[diagonal, column];
                    for (int column = 0; column < jacobianInv.columns; column++)
                        jacobianInv[row, column] -= value * jacobianInv[diagonal, column];

                }
            }
            
        }
        return jacobianInv;

    }

    void AssignJacobianIdentity(ref ArticulationJacobian jacobian)
    {
        for (int row = 0; row < jacobian.rows; row++)
        {
            for (int column = 0; column < jacobian.columns; column++)
                jacobian[row, column] = row != column ? 0.0f : 1.0f;
        }
    }

    // Does not work, for some reason solution is not converging.
    /*
    ArticulationJacobian GetDampedListSquaresJacobianMatrix(ArticulationJacobian jacobian, float lambda)
    {
        ArticulationJacobian jacobianT = JacobianTranspose(jacobian);
        ArticulationJacobian jjT = JacobianMultiply(jacobian, jacobianT);
        ArticulationJacobian identity = new ArticulationJacobian(jjT.rows, jjT.columns);
        AssignJacobianIdentity(ref identity);
        ArticulationJacobian inverseTerm = JacobianInverse(JacobianAdd(jjT, JacobianMultiply(identity, lambda * lambda)));
        ArticulationJacobian result = JacobianMultiply(jacobianT, inverseTerm);
        return result;
    }
    */
    void FixedUpdate()
    {
        if (!arBody || !arBody.isRoot)
            return;

        // nRows set to number of rows in matrix, which corresponds to the number of articulation links times 6.
        // nCols set to number of columns in matrix, which corresponds to the number of joint DOFs, plus 6 in the case eFIX_BASE is false.
        // Note that this computes the dense representation of an inherently sparse matrix.  Multiplication with this matrix maps 
        //joint space velocities to 6DOF world space linear and angular velocities.
        arBody.GetDofStartIndices(jointDofStarts);
        
        
        arBody.GetDenseJacobian(ref arJacobian);
        Vector3 targetPos = target.transform.position;
        Vector3 eePos = endEffector.transform.position;

        Vector3 dirToTarget = targetPos - eePos;
        
        List<float> jointSpacePositions = new List<float>();
        int totalDofs = arBody.GetJointPositions(jointSpacePositions);

        Vector3 targetRot = target.transform.rotation.eulerAngles;
        Vector3 eeRot = endEffector.transform.rotation.eulerAngles;

        Vector3 dirToTargetRot = targetRot - eeRot;
            
        var deltaPos = Time.fixedDeltaTime * positionAlpha * dirToTarget;
        var deltaRot = Time.fixedDeltaTime * rotationAlpha * dirToTargetRot;

        List<float> deltaTarget = new List<float>(arJacobian.rows);
        for (int i = deltaTarget.Count; i < arJacobian.rows - 6; i++)
            deltaTarget.Add(0.0f);
        deltaTarget.AddRange(new float[] {deltaPos.x, deltaPos.y, deltaPos.z, deltaRot.x, deltaRot.y, deltaRot.z});

        
        // Pretend that jacobian transpose is like an inverse.
        ArticulationJacobian jacobianT = JacobianTranspose(arJacobian);
        
        var deltaJointReducedSpace = JacobianMultiply(jacobianT, deltaTarget);
        for (int i = 0; i < totalDofs; i++)
        {
            jointSpacePositions[i] += deltaJointReducedSpace[i];
        }
        
        arBody.SetDriveTargets(jointSpacePositions);
    }
}
