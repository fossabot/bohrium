#!/usr/bin/env python
import sys

if len(sys.argv) > 1:
    import bohrium as np
else:
    import numpy as np

def test_range(n):
    a = np.arange(1,n+1)
    b = a
    c = a
    r = ((b+c)*b)/c
    return r

def test_range(n):
    a = np.arange(1,n+1)
    b = a[::2]
    c = a[::2]
    r = ((b+c)*b)/c
    return r

def test_random(n):
    a = np.random.random(n)
    b = a
    c = a
    r = ((b+c)*b)/c
    return r

def test_ones(n):
    a = np.ones((n,n,n,n),dtype=np.float32)
    #a = np.ones((n, n,n))
    b = a[::2]
    c = a[::2]
    
    return ((b+c)*b)/c

def test_reduce(n):
    #a = np.ones(1000).reshape(10,10,10)[:,:,::2]
    a = np.ones(900, dtype=np.float32).reshape(9,10,10)
    return np.add.reduce(a, axis=0) 
    #return np.add.reduce(b) 

if __name__ == "__main__":
    print(test_reduce(900))
    #print(test_range(20))
    #print(test_random(20))