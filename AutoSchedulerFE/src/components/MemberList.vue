<script setup lang="ts">
import { ref, watch } from 'vue';
import Separator from './ui/separator/Separator.vue';
import { storeToRefs } from 'pinia';
import Button from './ui/button/Button.vue';
import { useGroupStore } from '@/stores/groupStore';
import MemberForm from './MemberForm.vue';
import Table from './ui/table/Table.vue';
import TableHeader from './ui/table/TableHeader.vue';
import TableRow from './ui/table/TableRow.vue';
import TableHead from './ui/table/TableHead.vue';
import TableBody from './ui/table/TableBody.vue';
import TableCell from './ui/table/TableCell.vue';
import DialogContent from './ui/dialog/DialogContent.vue';
import Dialog from './ui/dialog/Dialog.vue';
import DialogTrigger from './ui/dialog/DialogTrigger.vue';

const showNewMemberModal = ref(false);
const groupStore = useGroupStore();
const {currentOrganizationIdx, members} = storeToRefs(groupStore);

watch(currentOrganizationIdx, ()=>{
    members.value = groupStore.organization(currentOrganizationIdx.value).value?.members??[];
});
</script>

<template>
    <Dialog>
      <DialogTrigger>
        <Button class="mx-10">New Member</Button>
      </DialogTrigger>
      <DialogContent>
        <MemberForm class="flex flex-col gap-5"/>
      </DialogContent>
    </Dialog>
    <div class="size-full border-box">
        <Table class="w-3/4 bg-secondary m-auto my-10 rounded-md border-box">
            <TableHeader>
                <TableRow>
                    <TableHead> Presenter name </TableHead>
                    <TableHead> Contacts </TableHead>
                    <TableHead> Delete </TableHead>
                </TableRow>
            </TableHeader>
            <TableBody>
                <TableRow v-for="member in members">
                    <TableCell>
                        {{member.name}}
                    </TableCell>
                    <TableCell>
                        {{member.contact}}
                    </TableCell>
                    <TableCell>
                        <Button @click="groupStore.removeMember(member.id)">Delete</Button>
                    </TableCell>
                </TableRow>
            </TableBody>
        </Table>
    </div>
</template>