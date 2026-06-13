<script setup lang="ts">
import { computed, onMounted, onUpdated, ref, watch } from 'vue';
import { storeToRefs } from 'pinia';
import { useActivityStore } from '@/stores/activityStore';
import Button from './ui/button/Button.vue';
import ActivityForm from './ActivityForm.vue';
import ActivityTypeForm from './ActivityTypeForm.vue';
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
import Tabs from './ui/tabs/Tabs.vue';
import TabsList from './ui/tabs/TabsList.vue';
import TabsTrigger from './ui/tabs/TabsTrigger.vue';
import TabsContent from './ui/tabs/TabsContent.vue';

const activityStore = useActivityStore();
const { activities, activityTypes } = storeToRefs(activityStore);
const showNewActivityModal = ref(false);
const groupStore = useGroupStore();
const {currentOrganizationIdx} = storeToRefs(groupStore);
//list could be made of expandible base types instead
const allTypes = computed(()=>activityTypes.value.flatMap(type => type.subtypes));

onMounted(()=>{
    activityStore.getActivitiesForOrganization(currentOrganizationIdx.value);
    activityStore.getActivityTypesForOrganization(currentOrganizationIdx.value);
});

watch(currentOrganizationIdx, ()=>{
    activityStore.getActivitiesForOrganization(currentOrganizationIdx.value);
    activityStore.getActivityTypesForOrganization(currentOrganizationIdx.value);
});
</script>

<template>
    <Tabs default-value="activities">
        <TabsList class="p-2 mx-10 bg-primary">
            <TabsTrigger class="bg-secondary mx-2" value="activities">
                Activities
            </TabsTrigger>
            <TabsTrigger class="bg-secondary mx-2" value="activityTypes">
                Activity Types
            </TabsTrigger>
        </TabsList>
        <TabsContent value="activities">
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
                            <TableHead> Type </TableHead>
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
                                {{activity.type?.baseTypeName}}{{ activity.type?.title?' - '+ activity.type?.title:'none' }}
                            </TableCell>
                            <TableCell>
                                <Button @click="activityStore.removeActivity(activity.id)">Delete</Button>
                            </TableCell>
                        </TableRow>
                    </TableBody>
                </Table>
            </div>
        </TabsContent>
        <TabsContent value="activityTypes">
            <Dialog>
                <DialogTrigger>
                    <Button class="mx-10">New Activity Type</Button>
                </DialogTrigger>
                <DialogContent>
                    <ActivityTypeForm class="flex flex-col gap-5"/>
                </DialogContent>
            </Dialog>
            <div class="size-full border-box">
                <Table class="w-3/4 bg-secondary m-auto my-10 rounded-md border-box">
                    <TableHeader>
                        <TableRow>
                            <TableHead> Base Type </TableHead>
                            <TableHead> Name of activity type </TableHead>
                            <TableHead> Description </TableHead>
                            <TableHead> Delete </TableHead>
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        <TableRow v-for="activityType in allTypes">
                            <TableCell>
                                {{activityType.baseTypeName}}
                            </TableCell>
                            <TableCell>
                                {{activityType.title}}
                            </TableCell>
                            <TableCell>
                                {{activityType.description}}
                            </TableCell>
                            <TableCell>
                                <Button @click="activityStore.removeActivityType(activityType.id)">Delete</Button>
                            </TableCell>
                        </TableRow>
                    </TableBody>
                </Table>
            </div>
        </TabsContent>
    </Tabs>
    
</template>