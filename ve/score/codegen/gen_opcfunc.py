#!/usr/bin/env python
from enums import *

switch_tmpl="""
cphvb_error cphvb_ve_score_execute(

    cphvb_intp          instruction_count,
    cphvb_instruction   instruction_list[]

) {

    for(cphvb_intp i=0; i<instruction_count; ++i) {

        cphvb_instruction   *instr  = &instruction_list[i];
        switch(instr->opcode) {

            case CPHVB_NONE:        // Nothing to do since we only use main memory.
            case CPHVB_DISCARD:
            case CPHVB_RELEASE:
            case CPHVB_SYNC:
                break;

            default:                // Element-wise functions + Memory Functions

                const long int poly = instr->opcode*100 + instr->operand[0]->type;

                switch(poly) {
                    __CASES__

                    default:                // Unsupported instruction
                        fprintf(
                            stderr, 
                            "cphvb_ve_score_execute() encountered an unknown opcode: %s.",
                            cphvb_opcode_text( instr->opcode )
                        );
                        exit(CPHVB_INST_NOT_SUPPORTED);

                }

        }

    }

    return CPHVB_SUCCESS;

}
"""

case_tmpl="""
case __OPCODE__*100+__ETYPE__:
    iter___OPCOUNT__<__TYPE__>( instr, &__FUNC__ );
    break;"""

func_tmpl="""
template <typename T>
cphvb_error __FUNC__(__PARAMS__ ) {
    // TODO: implement    
    return CPHVB_SUCCESS;
}
"""

def indent( text, c=1):
    return '\n'.join(['    '*c+line for line in text.split('\n')])

def main():

    func = ''    
    case = ''

#    for x in (x for x in (blank).split('\n') if x):
#        for t in (x for x in types.split('\n') if x and x not in ignore):
#            case += "case __OPCODE__*100+__ETYPE__:\n"\
#                    .replace('__OPCODE__', x)\
#                    .replace('__ETYPE__', t)
#    case += "    break;\n"

    for (count, opcodes) in opcode_map:
        for x in (x for x in opcodes.split('\n') if x):
            for t in (x for x in types.split('\n') if x and x not in ignore):
                case  += case_tmpl\
                        .replace('__OPCOUNT__', str(count))\
                        .replace('__OPCODE__', x)\
                        .replace('__ETYPE__', t)\
                        .replace('__FUNC__', x.lower().replace('cphvb', 'score'))\
                        .replace('__TYPE__', t.lower())
            func += func_tmpl\
                    .replace('__FUNC__', x.lower().replace('cphvb', 'score'))\
                    .replace('__PARAMS__', ','.join([" T *op%d"%i for i in xrange(1,count+1)]))
 
    print switch_tmpl.replace('__CASES__', indent(case, 5))
    #print func

if __name__ == "__main__":
    main()
