/*
This file is part of Bohrium and copyright (c) 2012 the Bohrium
team <http://www.bh107.org>.

Bohrium is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as 
published by the Free Software Foundation, either version 3 
of the License, or (at your option) any later version.

Bohrium is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the 
GNU Lesser General Public License along with Bohrium. 

If not, see <http://www.gnu.org/licenses/>.
*/

#include "ResourceManager.hpp"
#include <cassert>
#include <stdexcept>
#include <iostream>
#include <fstream>
#include <sstream>
#include <algorithm>
#include <cmath>

#ifdef _WIN32
#define STD_MIN(a, b) ((a) < (b) ? (a) : (b))
#define STD_MAX(a, b) ((a) >= (b) ? (a) : (b))
#else
#define STD_MIN(a, b) std::min(a, b)
#define STD_MAX(a, b) std::max(a, b)
#endif

ResourceManager::ResourceManager(bh_component* _component) 
    : component(_component)
{
    std::vector<cl::Platform> platforms;
    cl::Platform::get(&platforms);
    bool foundPlatform = false;
    for(std::vector<cl::Platform>::iterator pit = platforms.begin(); pit != platforms.end(); ++pit)        
    {
        try {
            cl_context_properties props[] = {CL_CONTEXT_PLATFORM, (cl_context_properties)(*pit)(),0};
            context = cl::Context(CL_DEVICE_TYPE_GPU, props);
            foundPlatform = true;
            break;
        } 
        catch (cl::Error)
        {
            foundPlatform = false;
        }
    }
    std::vector<std::string> extensions;
    if (foundPlatform)
    {
        devices = context.getInfo<CL_CONTEXT_DEVICES>();
        for(std::vector<cl::Device>::iterator dit = devices.begin(); dit != devices.end(); ++dit)        
        {
            commandQueues.push_back(cl::CommandQueue(context,*dit,
                                                     CL_QUEUE_OUT_OF_ORDER_EXEC_MODE_ENABLE
#ifdef BH_TIMING
                                                     | CL_QUEUE_PROFILING_ENABLE
#endif
                                        ));
            if (dit == devices.begin())
            {
                maxWorkGroupSize = dit->getInfo<CL_DEVICE_MAX_WORK_GROUP_SIZE>();
                maxWorkItemDims = dit->getInfo<CL_DEVICE_MAX_WORK_ITEM_DIMENSIONS>();
                maxWorkItemSizes = dit->getInfo<CL_DEVICE_MAX_WORK_ITEM_SIZES >();
            }
            else {
                size_t mwgs = dit->getInfo<CL_DEVICE_MAX_WORK_GROUP_SIZE>();
                maxWorkGroupSize = STD_MIN(maxWorkGroupSize,mwgs);
                cl_uint mwid = dit->getInfo<CL_DEVICE_MAX_WORK_ITEM_DIMENSIONS>();
                maxWorkItemDims = STD_MIN(maxWorkItemDims,mwid);
                std::vector<size_t> mwis = dit->getInfo<CL_DEVICE_MAX_WORK_ITEM_SIZES >();
                for (cl_uint d = 0; d < maxWorkItemDims; ++d)
                    maxWorkItemSizes[d] = STD_MIN(maxWorkItemSizes[d],mwis[d]);
            }
            extensions.push_back(dit->getInfo<CL_DEVICE_EXTENSIONS>());
        }
    } else {
        throw std::runtime_error("Could not find valid OpenCL platform.");
    }
    if (devices[0].getInfo<CL_DEVICE_ADDRESS_BITS>() == 64)
    {
        intpType_ = OCL_INT64;
    } else {
        intpType_ = OCL_INT32;
    }
    calcLocalShape();
    registerExtensions(extensions);

#ifdef BH_TIMING
    batchBuild = new bh::Timer<>("[GPU] Batch building");
    codeGen = new bh::Timer<>("[GPU] Code generation");
    kernelGen = new bh::Timer<>("[GPU] Kernel generation");
    bufferWrite = new bh::Timer<bh::timing4,1000000000>("[GPU] Writing buffers");
    bufferRead = new bh::Timer<bh::timing4,1000000000>("[GPU] Reading buffers");
    kernelExec = new bh::Timer<bh::timing4,1000000000>("[GPU] Kernel execution");
#endif
}

OCLtype ResourceManager::intpType()
{
    return intpType_;
}

#ifdef BH_TIMING
ResourceManager::~ResourceManager()
{
#ifdef STATIC_KERNEL
    std::cout << "---------------- STATS: STATIC_KERNEL -------------------" << std::endl;
#else
    std::cout << "---------------- STATS: DYNAMIC_KERNEL ------------------" << std::endl;
#endif
    delete batchBuild;
    delete codeGen;
    delete kernelGen;
    delete bufferWrite;
    delete bufferRead;
    delete kernelExec;
}
#endif


void ResourceManager::calcLocalShape()
{
    // Calculate "sane" localShapes
    size_t lsx = STD_MIN(256UL,maxWorkItemSizes[0]);
#ifdef DEBUG
    std::cout << "ResourceManager.localShape1D[" << lsx << "]" << std::endl;
#endif
    localShape1D.push_back(lsx);
    lsx = STD_MIN(32UL,maxWorkItemSizes[0]);
    size_t lsy = STD_MIN(maxWorkGroupSize/lsx,maxWorkItemSizes[1]);
    lsy /= 2;
#ifdef DEBUG
    std::cout << "ResourceManager.localShape2D[" << lsx << ", " << lsy << "]" << std::endl;
#endif
    localShape2D.push_back(lsx);
    localShape2D.push_back(lsy);
    lsx = STD_MIN(16UL,maxWorkItemSizes[0]);
    lsy = 1;
    while(lsy < std::sqrt((float)(maxWorkGroupSize/lsx)))
        lsy <<= 1;
    lsy = STD_MIN(lsy,maxWorkItemSizes[1]);
    size_t lsz = STD_MIN(maxWorkGroupSize/(lsx*lsy),maxWorkItemSizes[2]); 
    lsz /= 2;
#ifdef DEBUG
    std::cout << "ResourceManager.localShape3D[" << lsx << ", " << lsy << ", " << lsz << "]" << std::endl;
#endif
    localShape3D.push_back(lsx);
    localShape3D.push_back(lsy);
    localShape3D.push_back(lsz);
}

void ResourceManager::registerExtensions(std::vector<std::string> extensions)
{
    float64 = extensions[0].find("cl_khr_fp64") != std::string::npos;
#ifdef DEBUG
    std::cout << "ResourceManager.float64 = " << float64 << std::endl;
#endif
}

cl::Buffer* ResourceManager::createBuffer(size_t size)
{
    return new cl::Buffer(context, CL_MEM_READ_WRITE, size);
}

void ResourceManager::readBuffer(const cl::Buffer& buffer,
                                 void* hostPtr, 
                                 cl::Event waitFor,
                                 unsigned int device)
{
#ifdef DEBUG
    std::cout << "readBuffer(" << hostPtr << ")" << std::endl;
#endif
    size_t size = buffer.getInfo<CL_MEM_SIZE>();
    std::vector<cl::Event> readerWaitFor(1,waitFor);
#ifdef BH_TIMING
    cl::Event event;
#endif
    try {
        commandQueues[device].enqueueReadBuffer(buffer, CL_TRUE, 0, size, hostPtr, &readerWaitFor, 
#ifdef BH_TIMING
                                                &event
#else
                                                NULL
#endif
            );
    } catch (cl::Error e) {
        std::cerr << "[VE-GPU] Could not enqueueReadBuffer: \"" << e.err() << "\"" << std::endl;
    }
#ifdef BH_TIMING
    event.setCallback(CL_COMPLETE, &eventProfiler, bufferRead);
#endif
}

cl::Event ResourceManager::enqueueWriteBuffer(const cl::Buffer& buffer,
                                              const void* hostPtr, 
                                              std::vector<cl::Event> waitFor, 
                                              unsigned int device)
{
#ifdef DEBUG
    std::cout << "enqueueWriteBuffer(" << hostPtr << ")" << std::endl;
#endif
    cl::Event event;
    size_t size = buffer.getInfo<CL_MEM_SIZE>();
    try {
        commandQueues[device].enqueueWriteBuffer(buffer, CL_FALSE, 0, size, hostPtr, &waitFor, &event);
    } catch (cl::Error e) {
        std::cerr << "[VE-GPU] Could not enqueueWriteBuffer: \"" << e.what() << "\"" << std::endl;
        throw e;
    }
#ifdef BH_TIMING
    event.setCallback(CL_COMPLETE, &eventProfiler, bufferWrite);
#endif
    return event;
}

cl::Event ResourceManager::completeEvent()
{
    cl::UserEvent event(context);
    event.setStatus(CL_COMPLETE);
    return event;
}

cl::Kernel ResourceManager::createKernel(const std::string& source, 
                                          const std::string& kernelName)
{
    return createKernels(source, std::vector<std::string>(1,kernelName)).front();
}

void ResourceManager::buildKernels(const std::string& source,  
                                   void (CL_CALLBACK * notifyFptr)(cl_program, void *),void* kernelID,
                                   const std::string& options)
{
    cl::Program::Sources sources(1,std::make_pair(source.c_str(),source.size()));
    cl::Program program(context, sources);
    program.build(devices,(options+std::string(" ")+getIncludeStr()).c_str(),notifyFptr,kernelID);
}

std::vector<cl::Kernel> ResourceManager::createKernelsFromFile(const std::string& fileName, 
                                                               const std::vector<std::string>& kernelNames)
{
    std::ifstream file(fileName.c_str(), std::ios::in);
    if (!file.is_open())
    {
        throw std::runtime_error("Could not open source file.");
    }
    std::ostringstream source;
    source << file.rdbuf();
    return createKernels(source.str(), kernelNames);
}

std::vector<cl::Kernel> ResourceManager::createKernels(const std::string& source, 
                                                       const std::vector<std::string>& kernelNames)
{
#ifdef BH_TIMING
    bh_uint64 start = bh::Timer<>::stamp(); 
#endif

#ifdef DEBUG
    std::cout << "Program build :\n";
    std::cout << "------------------- SOURCE -----------------------\n";
    std::cout << source;
    std::cout << "------------------ SOURCE END --------------------" << std::endl;
#endif
    cl::Program::Sources sources(1,std::make_pair(source.c_str(),source.size()));
    cl::Program program(context, sources);
    try {
        program.build(devices,getIncludeStr().c_str());
    } catch (cl::Error) {
//#ifdef DEBUG
        std::cerr << "Program build error:\n";
        std::cerr << "------------------- SOURCE -----------------------\n";
        std::cerr << source;
        std::cerr << "------------------ SOURCE END --------------------\n";
        std::cerr << program.getBuildInfo<CL_PROGRAM_BUILD_LOG>(devices[0]) << std::endl;
//#endif
        throw std::runtime_error("Could not build Kernel.");
    }
    
    std::vector<cl::Kernel> kernels;
    for (std::vector<std::string>::const_iterator knit = kernelNames.begin(); knit != kernelNames.end(); ++knit)
    {
        try {
            kernels.push_back(cl::Kernel(program, knit->c_str()));
        } catch (cl::Error e) {
            std::cerr << "Could not create cl::Kernel " <<  knit->c_str() << ": " << e.what() << " " << 
                e.err() << std::endl;  
        }
    }
#ifdef BH_TIMING
    kernelGen->add({start, bh::Timer<>::stamp()});
#endif
    return kernels;
}

cl::Event ResourceManager::enqueueNDRangeKernel(const cl::Kernel& kernel, 
                                                const cl::NDRange& globalSize,
                                                const cl::NDRange& localSize,
                                                const std::vector<cl::Event>* waitFor,
                                                unsigned int device)
{
    cl::Event event;
    try 
    {
        commandQueues[device].enqueueNDRangeKernel(kernel, cl::NullRange, globalSize, localSize, waitFor, &event);
    } catch (cl::Error err)
    {
        std::cerr << "ERROR: " << err.what() << "(" << err.err() << ")" << std::endl;
        throw err;
    }
#ifdef BH_TIMING
    event.setCallback(CL_COMPLETE, &eventProfiler, kernelExec);
#endif
    //commandQueues[device].finish();
    return event;
}

std::vector<size_t> ResourceManager::localShape(const std::vector<size_t>& globalShape)
{
    switch (globalShape.size())
    {
    case 1:
        return localShape1D; 
    case 2:
        return localShape2D; 
    case 3:
        return localShape3D; 
    default:
        assert (false);
    }
}

bool ResourceManager::float64support()
{
    return float64;
}

#ifdef BH_TIMING
void CL_CALLBACK ResourceManager::eventProfiler(cl::Event event, cl_int eventStatus, void* timer)
{
    assert(eventStatus == CL_COMPLETE);
    ((bh::Timer<bh::timing4,1000000000>*)timer)->add({ event.getProfilingInfo<CL_PROFILING_COMMAND_QUEUED>(),
                event.getProfilingInfo<CL_PROFILING_COMMAND_SUBMIT>(),
                event.getProfilingInfo<CL_PROFILING_COMMAND_START>(),
                event.getProfilingInfo<CL_PROFILING_COMMAND_END>()});
}
#endif

std::string ResourceManager::getIncludeStr()
{
    char* dir = bh_component_config_lookup(component, "include");
    if (dir == NULL)
        return std::string("-I/opt/bohrium/gpu/include");
    return std::string("-I") + std::string(dir);
}

bh_error ResourceManager::childExecute(bh_ir* bhir)
{
    bh_error err = BH_ERROR;
    for (int i = 0; i < component->nchildren; ++i)
    {
        bh_component_iface* child = &component->children[i];
        err = child->execute(bhir);
        if (err == BH_SUCCESS)
            break;
    }
    return err;
}

