<script setup lang="ts">
import { onMounted, ref, watch } from 'vue';
import Separator from './ui/separator/Separator.vue';
import { storeToRefs } from 'pinia';
import { useActivityStore } from '@/stores/activityStore';
import Button from './ui/button/Button.vue';
import { useGroupStore } from '@/stores/groupStore';
import HallForm from './HallForm.vue';
import Dialog from './ui/dialog/Dialog.vue';
import DialogTrigger from './ui/dialog/DialogTrigger.vue';
import DialogContent from './ui/dialog/DialogContent.vue';
import Table from './ui/table/Table.vue';
import TableHeader from './ui/table/TableHeader.vue';
import TableRow from './ui/table/TableRow.vue';
import TableHead from './ui/table/TableHead.vue';
import TableBody from './ui/table/TableBody.vue';
import TableCell from './ui/table/TableCell.vue';
import AvailabilityList from './AvailabilityList.vue';
import type { Hall } from '@/classes/activity';
import type { Availability } from '@/classes/group';

const activityStore = useActivityStore();
const { halls } = storeToRefs(activityStore);
const showNewHallModal = ref(false);
const groupStore = useGroupStore();
const {currentOrganizationIdx} = storeToRefs(groupStore);

onMounted(()=>{
    halls.value = groupStore.organization(currentOrganizationIdx.value).value?.halls??[];
});

watch(currentOrganizationIdx, ()=>{
    halls.value = groupStore.organization(currentOrganizationIdx.value).value?.halls??[];
});

const handleAvailabilityChange = (hall:Hall, added:Availability)=>{
    hall.availability?.push({...added});
    activityStore.modifyHall(hall);
};
</script>

<template>
    <Dialog>
      <DialogTrigger>
        <Button class="mx-10">New Hall</Button>
      </DialogTrigger>
      <DialogContent>
        <HallForm class="flex flex-col gap-5"/>
      </DialogContent>
    </Dialog>
    <div class="size-full border-box">
        <Table class="w-3/4 bg-secondary m-auto my-10 rounded-md border-box">
            <TableHeader>
                <TableRow>
                    <TableHead> Hall </TableHead>
                    <TableHead> Description </TableHead>
                    <TableHead> Type </TableHead>
                    <TableHead> Availability </TableHead>
                    <TableHead> Delete </TableHead>
                </TableRow>
            </TableHeader>
            <TableBody>
                <TableRow v-for="hall in halls">
                    <TableCell>
                        {{hall.name}}
                    </TableCell>
                    <TableCell>
                        {{hall.description}}
                    </TableCell>
                    <TableCell>
                        {{hall.type?.title}}
                    </TableCell>
                    <TableCell>
                        <Dialog>
                            <DialogTrigger>
                                <Button class="mx-10">View</Button>
                            </DialogTrigger>
                            <DialogContent>
                                <AvailabilityList class="flex items-center gap-3" @added="(e)=>handleAvailabilityChange(hall, e)" :availability="hall.availability??[]"/>
                            </DialogContent>
                        </Dialog>
                    </TableCell>
                    <TableCell>
                        <Button @click="activityStore.removeHall(hall.id)">Delete</Button>
                    </TableCell>
                </TableRow>
            </TableBody>
        </Table>
    </div>
</template>