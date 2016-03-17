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

#include <bh_component.h>
#include <bh.h>
#include <bh_pprint.h>

using namespace std;

void noneremover_filter(bh_ir &bhir)
{
    bhir.instr_list.erase(
        std::remove_if(
            bhir.instr_list.begin(), bhir.instr_list.end(),
            [](bh_instruction instr) { return instr.opcode == BH_NONE; }
        ),
        bhir.instr_list.end()
    );
}
