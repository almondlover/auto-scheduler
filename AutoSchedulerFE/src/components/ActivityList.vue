<script setup lang="ts">
import { onMounted, onUpdated, ref, watch } from 'vue';
import Accordion from './ui/accordion/Accordion.vue';
import AccordionContent from './ui/accordion/AccordionContent.vue';
import AccordionItem from './ui/accordion/AccordionItem.vue';
import AccordionTrigger from './ui/accordion/AccordionTrigger.vue';
import Separator from './ui/separator/Separator.vue';
import { storeToRefs } from 'pinia';
import { useActivityStore } from '@/stores/activityStore';
import Button from './ui/button/Button.vue';
import ActivityForm from './ActivityForm.vue';
import { useGroupStore } from '@/stores/groupStore';
import Dialog from './ui/dialog/Dialog.vue';
import DialogTrigger from './ui/dialog/DialogTrigger.vue';
import DialogContent from './ui/dialog/DialogContent.vue';
import Table from './ui/table/Table.vue';
import TableHeader from './ui/table/TableHeader.vue';
import TableRow from './ui/table/TableRow.vue';
import TableHead from './ui/table/TableHead.vue';
import TableBody from './ui/table/TableBody.vue';
import TableCell from './ui/table/TableCell.vue';

const activityStore = useActivityStore();
const { activities } = storeToRefs(activityStore);
const showNewActivityModal = ref(false);
const groupStore = useGroupStore();
const {currentOrganizationIdx} = storeToRefs(groupStore);

onMounted(()=>{
    activityStore.getActivitiesForOrganization(currentOrganizationIdx.value);
});

watch(currentOrganizationIdx, ()=>{
    activityStore.getActivitiesForOrganization(currentOrganizationIdx.value);
});
</script>

<template>
    <Dialog>
      <DialogTrigger>
        <Button class="mx-10">New Activity</Button>
      </DialogTrigger>
      <DialogContent>
        <ActivityForm class="flex flex-col gap-5"/>
      </DialogContent>
    </Dialog>
    <div class="size-full border-box">
        <Table class="w-3/4 bg-secondary m-auto my-10 rounded-md border-box">
            <TableHeader>
                <TableRow>
                    <TableHead> Name of activity </TableHead>
                    <TableHead> Description </TableHead>
                    <TableHead> Delete </TableHead>
                </TableRow>
            </TableHeader>
            <TableBody>
                <TableRow v-for="activity in activities">
                    <TableCell>
                        {{activity.title}}
                    </TableCell>
                    <TableCell>
                        {{activity.description}}
                    </TableCell>
                    <TableCell>
                        <Button @click="activityStore.removeActivity(activity.id)">Delete</Button>
                    </TableCell>
                </TableRow>
            </TableBody>
        </Table>
    </div>
</template>