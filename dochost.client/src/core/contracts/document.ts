export type FileUploadInput = {
    files: FileList | null
}

export type DocumentInfo = {
    id: string
    fileName: string
    fileExt: string
    downloadCount: number
    createdAt: string
}